using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission
{
    public interface ISocket
    {
        bool IsRunning { get; }
        System.Net.Sockets.Socket TransferSocket { get; }
        void Close();
    }
}
