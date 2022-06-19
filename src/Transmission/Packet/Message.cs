using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Packet
{
    [Serializable]
    public class Message
    {
        [MaxLength(10200)]
        public string Body { get; set; }
        public DateTime SendTime { get; set; }
    }
}
