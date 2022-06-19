using ClientTransfer;
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
using WPFUI.MainWindows;

namespace WPFUI.StartUpWindows.Pages
{
    /// <summary>
    /// Interaction logic for ClientConnecting.xaml
    /// </summary>
    public partial class ClientConnecting : UserControl
    {
        Client _client;
        string _ip;
        string _port;
        Timer _aTimer;
        short _nextstate = 0;
        public ClientConnecting(string Ip , string Port)
        {
            InitializeComponent();
            _ip = Ip;
            _port = Port;
            Error.Text = $"نقطه نهایی اتصال Ip : {Ip} , Port : {Port} می باشد.";
            Error.Visibility = Visibility.Visible;
            InitializingClient();
        }

        public void InitializingClient()
        {
            _client = new Client(Disconnected_Handler);
            _aTimer = new Timer();
            _client.Connect(_ip, int.Parse(_port), Connected_Handler);
            _aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _aTimer.Interval = 1000;
            _aTimer.Enabled = true;
            _aTimer.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                switch (_nextstate)
                {
                    case 0: State.Text = "در حال اتصال"; _nextstate = 1; break;
                    case 1: State.Text = "در حال اتصال ."; _nextstate = 2; break;
                    case 2: State.Text = "در حال اتصال .."; _nextstate = 3; break;
                    case 3: State.Text = "در حال اتصال ..."; _nextstate = 0; break;
                    default: State.Text = "در حال اتصال"; _nextstate = 1; break;
                }
            });
        }

        private void Connected_Handler(object sender, string error)
        {
            if(string.IsNullOrEmpty(error))
            {
                Dispatcher.Invoke(() => {
                    MainWindow mainWindow = new MainWindow(null,_client);
                    mainWindow.Show();
                    Windows._CloseWindows();
                });
            }
            else
            {
                _aTimer.Stop();
                _nextstate = 0;
                Dispatcher.Invoke(() => {
                    State.Foreground = new SolidColorBrush(Colors.Red);
                    Error.Text = $"سروری با IP : {_ip} و Port : {_port} جهت اتصال یافت نشد ، لطفاً صحت اطلاعات را بررسی کنید و دوباره تلاش کنید.";
                    State.Text = "خطا در اتصال";
                    Error.Visibility = Visibility.Visible;
                    Retry.Visibility = Visibility.Visible;
                });
            }
        }

        private void Disconnected_Handler(object sender)
        {
            _aTimer.Stop();
            Dispatcher.Invoke(() => {
                Dispatcher.Invoke(() => {
                    State.Foreground = new SolidColorBrush(Colors.Red);
                    Error.Text = $"سروری با IP : {_ip} و Port : {_port} جهت اتصال یافت نشد ، لطفاً صحت اطلاعات را بررسی کنید و دوباره تلاش کنید.";
                    State.Text = "خطا در اتصال";
                    Error.Visibility = Visibility.Visible;
                    Retry.Visibility = Visibility.Visible;
                });
            });
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _aTimer.Stop();
            _aTimer.Dispose();
            _client.Close();
            ClientInit clientInit = new ClientInit(_port,_ip);
            WPFUI.StartUpWindows.Windows._changePage(clientInit);
        }

        private void Retry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClientConnecting connecting = new ClientConnecting(_ip, _port);
                WPFUI.StartUpWindows.Windows._changePage(connecting);
            }
            catch (Exception)
            {
                MessageBox.Show("اطلاعات وارد شده مشکلی دارد ، لطفاً اطلاعات را تصحیح کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
