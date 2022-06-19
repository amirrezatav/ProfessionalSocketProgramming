using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Transmission;

namespace Listener
{
    public delegate void SocketAcceptedHandler(object sender, AcceptedSocket e);

    public class Server : ISocket
    {
        private Socket _serverSocket = null;
        private int _serverPort = -1;
        private string _serverIp = "";
        private bool _isRunning = false;

        private event SocketAcceptedHandler _accepteHandler;
       

        public bool IsRunning
        { 
            get { return _isRunning; } 
        }
        public int Port
        {
            get { return _serverPort; }
        }
        public string Ip
        {
            get { return _serverIp; }
        }
        public Socket BaseSocket
        {
            get { return _serverSocket; }
        }

        public Socket TransferSocket { get; private set; }

        public Server(SocketAcceptedHandler handler)
        {
            _accepteHandler = handler;
        }


        public static Task<List<string>> GetAllIpAsync()
        {
            List<string> list = new List<string>();
            string myHost = System.Net.Dns.GetHostName();

            System.Net.IPHostEntry myIPs = System.Net.Dns.GetHostEntry(myHost);

            foreach (var item in myIPs.AddressList)
            {
                if(item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    list.Add(item.ToString());
            }

            return Task.FromResult(list);
        }

        public void Start(string ip , int port)
        {
            if (_isRunning)
                return;
            _serverPort = port;
            _serverIp = ip;
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(_serverIp), _serverPort));
            _serverSocket.Listen(1);
            _isRunning = true;
            _serverSocket.BeginAccept(callback,null);
        }
        public void StartReAccept(SocketAcceptedHandler handler)
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse(_serverIp), _serverPort));
            _serverSocket.Listen(1);
            _isRunning = true;
            _serverSocket.BeginAccept(callback, null);
            _accepteHandler = handler;
        }

        private void callback(IAsyncResult ar)
        {
            try
            {
                Socket sck = _serverSocket.EndAccept(ar);
                if (sck != null)
                {
                    if (TransferSocket == null || !TransferSocket.Connected)
                    {
                        TransferSocket = sck;
                        _accepteHandler(this, new AcceptedSocket(sck));
                        return;
                    }
                }
            }
            catch 
            {
                _isRunning = false;
                Close();
            }
            if (_isRunning)
                _serverSocket.BeginAccept(callback, null);
            else return;
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            Close();

            _isRunning = false;
        }

        public void Close()
        {
            if (TransferSocket != null)
            {
                TransferSocket.Close();
                TransferSocket.Dispose();
            }

            if (_serverSocket != null)
            {
                _serverSocket.Close();
                _serverSocket.Dispose();
            }
        }
    }
}
