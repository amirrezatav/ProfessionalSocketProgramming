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
    public class UploadQueue : BaseQueue
    {
        private bool _running;
        private Transfer _transfer;
        private Thread thread;

        public UploadQueue(Transfer transfer, string filepath , ProgressChange progressChange)
        {
            _filePath = filepath;
            var info = new System.IO.FileInfo(filepath);
            _fileInfo = new Transmission.Packet.FileInfo()
            {
                Extention = info.Extension,
                Name = Path.GetFileNameWithoutExtension(info.Name),
                Size = info.Length,
                SendTime = DateTime.Now,
                
            };
            _progressChanged += progressChange;
            _transfer = transfer;
            _fileStream = new FileStream(filepath, FileMode.Open);
            _id = new Random().Next();
            while(ProcessPacket.Queues.ContainsKey(_id))
                _id = new Random().Next();
            thread = new Thread(new ParameterizedThreadStart(Sending));
            thread.IsBackground = true;
        }
        public void SendFileInfo()
        {
            Head head = new Head()
            {
                Id = _id,
                Type = PacketType.FileInfo
            };

            PacketSerializer packetSerializer = new PacketSerializer();
            packetSerializer.Serialize(head);
            packetSerializer.Serialize(_fileInfo);
            _transfer.Send(packetSerializer.GetByte());
        }

        public void Start()
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


            while(uploadQueue._running && _fileindex < uploadQueue._fileInfo.Size)
            {
                uploadQueue._fileStream.Position = _fileindex;

                FileBody fileBody = new FileBody();
                // 10240 - 5 - 8 =  10232
                int red = uploadQueue._fileStream.Read(fileBody.Body,0, PacketConfig.FileBody);
                fileBody.Count = red;
                fileBody.Index = _fileindex;

                Head head = new Head() {
                    Id = uploadQueue._id, // 4
                    Type = PacketType.FileBody // 1
                };

                PacketSerializer packetSerializer = new PacketSerializer();
                packetSerializer.Serialize(head);
                packetSerializer.Serialize(fileBody);

                int size = packetSerializer.GetByte().Length;

                uploadQueue._transfer.Send(packetSerializer.GetByte());


                uploadQueue.Trasfered += red;
                uploadQueue._fileindex += red;

                uploadQueue.Progress = (int)((uploadQueue.Trasfered * 100) / uploadQueue._fileInfo.Size);
                if(uploadQueue.LastProgress < uploadQueue.Progress)
                {
                    uploadQueue.LastProgress = uploadQueue.Progress;
                    uploadQueue._progressChanged(this);
                }

                Thread.Sleep(10);
            }

            uploadQueue.Close();
        }
    }
}
