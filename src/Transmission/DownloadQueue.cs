using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmission.Packet;

namespace Transmission
{
    public class DownloadQueue : BaseQueue
    {

        public DownloadQueue(int id,
            Transmission.Packet.FileInfo fileInfo,
            ProgressChange progressChange)
        {
            _progressChanged += progressChange;
            _fileInfo = fileInfo;
            _filePath = Path.Combine(RootPath, fileInfo.Name + ".tmp");
            try
            {
                _fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write);
            }
            catch (Exception)
            {
                File.Delete(_filePath);
                _fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.Write);
            }
            _id = id;
        }
        public void Close()
        {
            _fileStream.Close();
        }
        public void Write(FileBody fileBody)
        {
            _fileStream.Position = fileBody.Index;
            _fileindex = fileBody.Index;
            _fileStream.Write(fileBody.Body, 0, fileBody.Count);
            Trasfered += fileBody.Count;
            Progress = (int)((Trasfered * 100) / _fileInfo.Size);
            if (LastProgress < Progress)
            {
                LastProgress = Progress;
                if(LastProgress == 100)
                {
                    Close();
                    _filePath = Path.Combine(RootPath, _fileInfo.Name + _fileInfo.Extention);

                    while(File.Exists(_filePath))
                        _filePath = Path.Combine(RootPath, _fileInfo.Name + $" ({new Random().Next() })"+ _fileInfo.Extention);

                    System.IO.File.Move(Path.Combine(RootPath, _fileInfo.Name + ".tmp"), _filePath);
                }
                _progressChanged(this);
            }
        }
    }
}
