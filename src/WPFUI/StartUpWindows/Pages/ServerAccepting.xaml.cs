using Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Transmission;
using WPFUI.MainWindows;

namespace WPFUI.StartUpWindows.Pages
{
    /// <summary>
    /// Interaction logic for Connecting.xaml
    /// </summary>
    public partial class ServerAccepting : UserControl
    {
        Server _server;
        string _ip;
        string _port;
        Timer _aTimer;
        short _nextstate = 0;
        public ServerAccepting(string Ip, string Port)
        {
            InitializeComponent();
            _ip = Ip;
            _port = Port;
            InitializingServer();
            Info.Text = $"برای اتصال از Ip : {_ip} و Port : {_port} استفاده کنید.";
        }

        private void SocketAccepted_Handler(object sender, AcceptedSocket e)
        {
            Dispatcher.Invoke(() =>{
                MainWindow mainWindow = new MainWindow(_server,null);
                mainWindow.Show();
                Windows._CloseWindows();
            });
        }

        public void InitializingServer()
        {
            try
            {
                _server = new Server(SocketAccepted_Handler);
                _server.Start(_ip, int.Parse(_port));
                _aTimer = new Timer();
                _aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                _aTimer.Interval = 1000;
                _aTimer.Enabled = true;
                _aTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"خطایی رخ داده است",MessageBoxButton.OK,MessageBoxImage.Error);
                _server.Close();
                if(_aTimer != null)
                _aTimer.Stop();
                Retry.Visibility = Visibility.Visible;
                throw;
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {

            Dispatcher.Invoke(() => {
                switch (_nextstate)
                {
                    case 0: State.Text = "در انتظار اتصال"; _nextstate = 1; break;
                    case 1: State.Text = "در انتظار اتصال ."; _nextstate = 2; break;
                    case 2: State.Text = "در انتظار اتصال .."; _nextstate = 3; break;
                    case 3: State.Text = "در انتظار اتصال ..."; _nextstate = 0; break;
                    default: State.Text = "در انتظار اتصال"; _nextstate = 1; break;
                }
            });
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _server.Close();
            _aTimer.Stop();
            _aTimer.Dispose();
            ServerInit serverInit = new ServerInit();
            WPFUI.StartUpWindows.Windows._changePage(serverInit);
        }

        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            _server = new Server(SocketAccepted_Handler);
            _server.Start(_ip, int.Parse(_port));
            _aTimer.Start();
        }
    }
}
