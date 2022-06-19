using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Listener
{
    public class AcceptedSocket
    {
        public Socket Accepted { get; private set; }
        public IPAddress Address { get; private set; }
        public int Port { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public AcceptedSocket(Socket socket)
        {
            Accepted = socket;
            Address = ((IPEndPoint)Accepted.RemoteEndPoint).Address;
            Port = ((IPEndPoint)Accepted.RemoteEndPoint).Port;
            EndPoint = (IPEndPoint)Accepted.RemoteEndPoint;
        }
        public override string ToString()
        {
            var res = $"Ip : {Address.ToString()}\n" +
                $"Port : {Port}";
            return res;
        }
    }
}
