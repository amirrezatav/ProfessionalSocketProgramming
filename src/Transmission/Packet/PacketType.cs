using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Packet
{
    public enum PacketType : byte
    {
        Message,
        FileInfo,
        FileBody,
        Start
    }
}
