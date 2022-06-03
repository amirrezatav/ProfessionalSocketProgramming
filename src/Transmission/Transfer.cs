using System;
using System.Net.Sockets;

namespace Transmission
{
    public class Transfer
    {
        private const int _bufferSize = 1024 * 5;
        private byte[] _buffer;
        private ISocket _socket;
        public Transfer(ISocket socket, bool canTransfer)
        {
            _socket = socket;
        }
        public void Send(byte[] buffer)
        {
            if (!_socket.IsRunning)
                return;
            _socket.TransferSocket.Send(buffer,0,buffer.Length,SocketFlags.None);
        }
        public void RunReceive()
        {
            _socket.TransferSocket.BeginReceive(_buffer, 0, _bufferSize, SocketFlags.Peek, receiveCallback, null);
        }
        private void receiveCallback(IAsyncResult ar)
        {
            try
            {
                int ReceiveSize = _socket.TransferSocket.EndReceive(ar);
                if (ReceiveSize >= 4)
                {
                    _socket.TransferSocket.Receive(_buffer);
                    ProcessPacket.Process(_buffer);
                }
                RunReceive();
            }
            catch (Exception ex)
            {
                _socket.Close();
                throw ex;
            }
        }
    }
}
