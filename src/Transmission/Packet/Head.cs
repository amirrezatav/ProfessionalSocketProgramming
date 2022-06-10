using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Packet
{
    /// <summary>
    /// Header Packet - 9 byte
    /// </summary>
    /// 
    [Serializable]
    public class Head
    {
        /// <summary>
        /// Packet Size
        /// </summary>
        public int Size { get; set; } // 4 byte
        /// <summary>
        /// Packet Id
        /// </summary>
        public int Id { get; set; } // 4 byte
        /// <summary>
        /// Type of packet
        /// </summary>
        public PacketType Type { get; set; } // 1 byte
    }
}
