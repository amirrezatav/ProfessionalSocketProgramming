using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmission
{
    public delegate void ProgressChange(BaseQueue queue);
    public class BaseQueue
    {
        protected const string RootPath = PacketConfig.DowloadPath;
        protected string _filePath;
        protected Transmission.Packet.FileInfo _fileInfo;
        protected FileStream _fileStream;
        protected int _id;

        protected int _fileindex = 0;
        protected long Trasfered = 0;
        protected int Progress = 0;
        protected int LastProgress = 0;

        protected ProgressChange _progressChanged;
        public int Id { get { return _id; } }
        public int ProgressFile { get { return LastProgress; } }
        public string FilePath { get { return _filePath; } }
        public Transmission.Packet.FileInfo FileInfo { get { return _fileInfo; } }


    }
}
