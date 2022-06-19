using ClientTransfer;
using Listener;
using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Transmission;
using Transmission.Packet;
using Transmission.Packet.PacketSerialize;
using WPFUI.MainWindows.Message;

namespace WPFUI.MainWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Transfer _transfer;
        private Server _server = null;
        private Client _cleint = null;

        public MainWindow(Server server,Client cleint)
        {
            InitializeComponent();
            if(server != null)
            {
                _transfer = new Transfer(server, true, Message_Handler, File_Handler , connection_Failed);
                _server = server;
                ShowConfigMessage("متصل شدید.\n اطلاعات متصل شده به شرح زیر است.", new AcceptedSocket(_server.TransferSocket).ToString(), Colors.Green);

                Title = "Sharing Server";
            }
            else
            {
                _transfer = new Transfer(cleint, true, Message_Handler, File_Handler, connection_Failed);
                _cleint = cleint;
                ShowConfigMessage("متصل شدید.\n اطلاعات شما به شرح زیر است.", new AcceptedSocket(_cleint.ClientSocket).ToString(), Colors.Green);
                Title = "Sharing Cleint";
            }
            _transfer.RunReceive();
        }

        private void ShowConfigMessage(string message,string Config , Color color)
        {
            ChatPanel.Children.Clear();
            TextBlock text = new TextBlock();
            text.FlowDirection = FlowDirection.RightToLeft;
            text.TextAlignment = TextAlignment.Center;
            text.Text = message;
            text.Text += "\n"+Config;
            text.Margin = new Thickness(10);
            text.Foreground = new SolidColorBrush(color);
            text.FontWeight = FontWeights.ExtraBold;
            text.FontSize = 18;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            ChatPanel.Children.Add(text);
        }

        private void connection_Failed(Exception error)
        {
            if(_cleint != null)
            {
                var res = System.Windows.MessageBox.Show(error.Message + "\nمیخواهید دوباره متصل شوید ؟", "خطا در اتصال", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if(res == MessageBoxResult.Yes)
                {
                    Dispatcher.Invoke(() => {
                        StartUpWindows.Windows windows = new StartUpWindows.Windows();
                        windows.Show();
                        this.Close();
                    });
                }
                
            }
            else if(_server != null)
            {
                _server.StartReAccept(SocketAccepted_Handler);
                Dispatcher.Invoke(() => {
                    ShowConfigMessage("اتصال قطع شد. در انتظار اتصال مجدد.", $"برای اتصال از IP {_server.Ip} و Port : {_server.Port} استفاده کنید.", Colors.Red);
                });
            }
        }

        private void SocketAccepted_Handler(object sender, AcceptedSocket e)
        {
            _server = (Server)sender;
            _transfer = new Transfer(_server, true, Message_Handler, File_Handler, connection_Failed);
            _transfer.RunReceive();
            Dispatcher.Invoke(() => {
                ShowConfigMessage("متصل شدید.\n اطلاعات متصل شده به شرح زیر است.", new AcceptedSocket(_server.TransferSocket).ToString(), Colors.Green);
            });
        }

        private DownloadQueue File_Handler(FileInfo fileInfo,int id)
        {
            DownloadQueue downloadQueue = null;
            Dispatcher.Invoke(() => {
                FileMessage file = new FileMessage();
                downloadQueue = file.AddDownloadFile(fileInfo,id);
                AddMessageToUi(file);
            });
            return downloadQueue;
        }

        private void Message_Handler(Transmission.Packet.Message message)
        {
            Dispatcher.Invoke(() => {
                TextMessage textMessage = new TextMessage(message.Body, message.SendTime.ToShortTimeString());
                textMessage.HorizontalAlignment = HorizontalAlignment.Left;
                AddMessageToUi(textMessage);

            });
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    if(!string.IsNullOrEmpty(MessageBox.Text.Trim()))
                    {
                        Transmission.Packet.Message message = new Transmission.Packet.Message()
                        {
                            Body = MessageBox.Text.Trim(),
                            SendTime = DateTime.Now
                        };

                        Head head = new Head()
                        {
                            Id = new Random().Next(),
                            Type = PacketType.Message
                        };

                        PacketSerializer packetSerializer = new PacketSerializer();
                        packetSerializer.Serialize(head);
                        packetSerializer.Serialize(message);
                        _transfer.Send(packetSerializer.GetByte());
                        TextMessage textMessage = new TextMessage(message.Body, message.SendTime.ToShortTimeString());
                        textMessage.HorizontalAlignment = HorizontalAlignment.Right;
                        AddMessageToUi(textMessage);

                        MessageBox.Text = "";
                    }
                }
            }
        }

        private void Send_File(string path)
        {
            Dispatcher.Invoke(() => {
            FileMessage file = new FileMessage(_transfer, path);
                AddMessageToUi(file);
            });
        }

        private void AllFile_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    Send_File(filename);
                    Thread.Sleep(300);
                }
            }
        }

        private void AddMessageToUi(object o)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => {
                    ChatPanel.Children.Add((UIElement)o);
                    ChatPanelScroll.ScrollToEnd();
                });
                return;
            }
            else
            {
                ChatPanel.Children.Add((UIElement)o);
                ChatPanelScroll.ScrollToEnd();
            }
        }

        private void Photos_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Jpeg files (*.jpg)|*.jpg|Png files (*.png)|*.png|Tif files (*.tif)|*.tif|Gif files (*.gif)|*.gif|Svg files (*.svg)|*.svg|Bitmap files (*.bitmap)|*.bitmap";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    Send_File(filename);
                    Thread.Sleep(100);
                }
            }
        }

        private void Sounds_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Mp3 files (*.mp3)|*.mp3|AAC files (*.aac)|*.aac|Ogg files (*.ogg)|*.ogg|Wav files (*.wav)|*.wav|Mp2 files (*.mp2)|*.mp2";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    Send_File(filename);
                    Thread.Sleep(300);
                }
            }
        }

        private void Vidoes_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Mp4 files (*.mp4)|*.mp4|MOV files (*.mov)|*.mov|Mkv files (*.mkv)|*.mkv|Mpg files (*.mpg)|*.mpg|Avi files (*.avi)|*.avi";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    Send_File(filename);
                    Thread.Sleep(300);
                }
            }
        }

        private void Info_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock text = new TextBlock();
            text.FlowDirection = FlowDirection.RightToLeft;
            text.TextAlignment = TextAlignment.Center;
            text.Text = "طراحی شده توسط امیررضا توکلی";
            text.Text += "\n" + "Amirrezatav2@gmail.com";
            text.Text += "\n" + "Github.com/Amirrezatav";
            text.Text += "\n" + "Telegram.com/Amirrezatav";
            text.Text += "\n" + "Instagram.com/Amirrezatav5";
            text.Text += "\n" + "98-935-749-1027";
            text.Text += "\n" + "2022-1401-IRI";
            text.Text += "\n" + "Shahed Univercity";
            text.Margin = new Thickness(10);
            text.Foreground = new SolidColorBrush(Colors.DarkBlue);
            text.FontWeight = FontWeights.ExtraBold;
            text.FontSize = 18;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            ChatPanel.Children.Add(text);
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string filename in files)
                {
                    Send_File(filename);
                    Thread.Sleep(300);
                }
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(MessageBox);
        }
        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(MessageBox);
        }
    }
}
