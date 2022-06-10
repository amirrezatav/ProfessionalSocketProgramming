using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transmission.Packet;
using Transmission.Packet.PacketSerialize;

namespace Transmission
{
    public class UploadQueue
    {
        private bool _running;
        private Transmission.Packet.FileInfo _fileInfo;
        private Transfer _transfer;
        private int Id;
        private Thread thread;
        private FileStream _fileStream;


        private int fileindex = 0;

        public UploadQueue(Transfer transfer, string filepath)
        {
            _running = true;
            var info = new System.IO.FileInfo(filepath);
            _fileInfo = new Transmission.Packet.FileInfo()
            {
                Extention = info.Extension,
                Name = info.Name,
                Size = info.Length
            };
            _transfer = transfer;
            _fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            Id = new Random().Next();
            thread = new Thread(new ParameterizedThreadStart(Sending));
            thread.IsBackground = true;
        }

        private void Start()
        {
            _running = true;
            thread.Start(this);
        }
        public void Close()
        {
            _running = false;
            _fileStream.Close();
        }
        private void Sending(object o)
        {
            UploadQueue uploadQueue = (UploadQueue)o;

            while(uploadQueue._running && fileindex <= uploadQueue._fileInfo.Size)
            {
                uploadQueue._fileStream.Position = fileindex;

                FileBody fileBody = new FileBody();

                int red = uploadQueue._fileStream.Read(fileBody.Body,0, 10223);
                fileBody.Count = red;
                fileBody.Index = fileindex;

                Head head = new Head() {
                Id = uploadQueue.Id,
                Size = 0,
                Type = PacketType.FileBody
                };

                PacketSerializer packetSerializer = new PacketSerializer();
                packetSerializer.Serialize(head);
                packetSerializer.Serialize(fileBody);

                uploadQueue._transfer.Send(packetSerializer.GetByte());

            }
        }
    }
}
