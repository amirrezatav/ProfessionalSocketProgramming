using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Packet
{
    [Serializable]
    public class FileInfo
    {
        public string Name { get; set; }
        public string Extention { get; set; }
        public long Size { get; set; }
        public DateTime SendTime { get; set; }
    }
}
