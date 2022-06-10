using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Packet.PacketSerialize
{
    public class PacketDeserializer : BinaryReader
    {
        private BinaryFormatter _binaryFormatter;

        public PacketDeserializer(byte[] buffer) 
            : base(new MemoryStream(buffer))
        {
            _binaryFormatter = new BinaryFormatter();
        }

        public T DeSerialize<T>()
        {
            return (T)_binaryFormatter.Deserialize(BaseStream);
        }
       
    }
}
