using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Transmission.Packet.PacketSerialize
{
    public class PacketDeserializer : BinaryReader
    {
        private BinaryFormatter _binaryFormatter;

        public PacketDeserializer(byte[] buffer, int count) 
            : base(new MemoryStream(buffer,0, count))
        {
            _binaryFormatter = new BinaryFormatter();
        }

        public T DeSerialize<T>()
        {

            var res = (T)_binaryFormatter.Deserialize(BaseStream);
            return res;
        }
       
    }
}
