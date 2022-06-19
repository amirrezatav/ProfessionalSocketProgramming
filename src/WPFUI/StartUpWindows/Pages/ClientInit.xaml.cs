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

namespace WPFUI.StartUpWindows.Pages
{
    /// <summary>
    /// Interaction logic for ClientInit.xaml
    /// </summary>
    public partial class ClientInit : UserControl
    {
        public ClientInit()
        {
            InitializeComponent();
        }

        public ClientInit(string port , string ip)
        {
            InitializeComponent();
            Port.Text = port;
            Ip.Text = ip;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClientConnecting connecting = new ClientConnecting(Ip.Text, Port.Text);
                WPFUI.StartUpWindows.Windows._changePage(connecting);
            }
            catch (Exception)
            {
                MessageBox.Show("اطلاعات وارد شده مشکلی دارد ، لطفاً اطلاعات را تصحیح کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ApplicationType ApplicationType = new ApplicationType();
            WPFUI.StartUpWindows.Windows._changePage(ApplicationType);
        }
    }
}
