using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using Transmission;

namespace WPFUI.MainWindows.Message
{
    /// <summary>
    /// Interaction logic for FileMessage.xaml
    /// </summary>
    public partial class FileMessage : UserControl
    {
        private string _filePath;
        private int _preogress = 0;
        private int _id;
        /// <summary>
        /// Use for download and then use <see cref="AddDownloadFile"/>
        /// </summary>
        public FileMessage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Use for upload
        /// </summary>
        /// <param name="trasfer"></param>
        /// <param name="filepath"></param>
        public FileMessage(Transfer trasfer,string filepath)
        {
            InitializeComponent();
            _filePath = filepath;
            UploadQueue upload = new UploadQueue(trasfer, _filePath, Progress_Changed);
            upload.SendFileInfo();
            _id = upload.Id;
            Name.Text = upload.FileInfo.Name + upload.FileInfo.Extention;
            Time.Text = upload.FileInfo.SendTime.ToShortTimeString();
            HorizontalAlignment = HorizontalAlignment.Right;
            ProcessPacket.Queues.Add(upload.Id, upload);
        }

        public DownloadQueue AddDownloadFile(Transmission.Packet.FileInfo fileInfo,int id)
        {
            DownloadQueue downloadQueue = new DownloadQueue(id, fileInfo, Progress_Changed);
            _id = downloadQueue.Id;
            _filePath = downloadQueue.FilePath;
            Name.Text = fileInfo.Name + fileInfo.Extention;
            Time.Text = fileInfo.SendTime.ToShortTimeString();
            HorizontalAlignment = HorizontalAlignment.Left;
            AttachBtn.IsEnabled = false;
            ProcessPacket.Queues.Add(_id, downloadQueue);
            return downloadQueue;
        }

        private void Progress_Changed(BaseQueue queue)
        {
            if (queue != null)
            {
                ProgressBar(queue.ProgressFile);
                if (queue.ProgressFile == 100)
                {
                    _filePath = queue.FilePath;
                    ProcessPacket.Queues.Remove(queue.Id);
                    Dispatcher.Invoke(() => {
                        AttachBtn.IsEnabled = true;
                        ButtonProgressAssist.SetOpacity(AttachBtn, 0);
                    });
                }
            }
        }

        public void ProgressBar(int value)
        {
            Dispatcher.Invoke(() => { 
                ButtonProgressAssist.SetValue(AttachBtn, value);
                _preogress = value;
            });
        }

        private void AttachBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(_filePath);
            }
            catch 
            {
                AttachBtn.IsEnabled = false;
                MessageBox.Show("فایل مورد نظرا یافت نشد.", "اخطار",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }

        private void AttachBtn_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            if (_preogress != 100)
                AttachBtn.ToolTip = $"{_preogress} درصد منتقل شده است.";
            else AttachBtn.ToolTip = "فایل کامل منتقل شده است.";
        }
    }
}
