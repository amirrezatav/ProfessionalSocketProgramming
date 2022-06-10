using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmission.Packet;
using Transmission.Packet.PacketSerialize;

namespace Transmission
{
    public delegate void Message_Handler(Message message);

    public class ProcessPacket
    {
        private event Message_Handler _messageHandler;
        public ProcessPacket(Message_Handler messageHandler)
        {
            _messageHandler = messageHandler;
        }


        public Task Process(byte[] _buffer)
        {
            PacketDeserializer deserializer = new PacketDeserializer(_buffer);
            Head head = deserializer.DeSerialize<Head>();
            switch (head.Type)
            {
                case PacketType.Message:
                    {
                        Message message = deserializer.DeSerialize<Message>();
                        _messageHandler(message);
                        break;
                    }
                default:
                    throw new Exception("اطلاعات دریافت شده دارای مشکل است.");
            }
            return Task.CompletedTask;
        }
    }
}
