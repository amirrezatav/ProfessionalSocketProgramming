using ClientTransfer;
using Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmission;
using Transmission.Packet;
using Transmission.Packet.PacketSerialize;

namespace ConsoleUI
{
    public class Program
    {
        static Transfer Transmission;
        static Server server;
        static Client client;
        static void Main(string[] args)
        {
            
            Console.Write("1- Server or 2- Client ?");
            if(int.Parse(Console.ReadLine()) == 1)
            {
                server = new Server(accepterhandler);
                server.Start("127.0.0.1",8080);
            }
            else
            {
                client = new Client(disconnectedHandler);
                client.Connect("127.0.0.1", 8080, connectedHandler);
            }
            Console.ReadLine();
        }
        
        private static void connectedHandler(object sender, string error)
        {
            Transmission = new Transfer(client, true, message_Handler);
            Transmission.RunReceive();
        }

        private static void disconnectedHandler(object sender)
        {
            throw new NotImplementedException();
        }

        // server

        private static void accepterhandler(object sender, AcceptedSocket e)
        {
            Transmission = new Transfer(server, true,message_Handler);


            Message message = new Message() { 
            Body = "Hello From Server.",
            SendTime = DateTime.Now
            };


            Head head = new Head()
            { 
            Id = new Random().Next(),
            Size = 0,
            Type = PacketType.Message
            };


            PacketSerializer serializer = new PacketSerializer();
            serializer.Serialize(head);
            serializer.Serialize(message);
            Transmission.Send(serializer.GetByte());
        }

        private static void message_Handler(Message message)
        {
            Console.WriteLine(message.Body);

            Message mmessage = new Message()
            {
                Body = "Hello From Cleint.",
                SendTime = DateTime.Now
            };


            Head head = new Head()
            {
                Id = new Random().Next(),
                Size = 0,
                Type = PacketType.Message
            };


            PacketSerializer serializer = new PacketSerializer();
            serializer.Serialize(head);
            serializer.Serialize(mmessage);
            Transmission.Send(serializer.GetByte());
        }
    }
}
