using Listener;
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
    /// Interaction logic for ServerInit.xaml
    /// </summary>
    public partial class ServerInit : UserControl
    {
        public ServerInit()
        {
            InitializeComponent();
            SetIp();
        }

        async void SetIp()
        {
            var res = await Server.GetAllIpAsync();
            foreach (var item in res)
            {
                Ip.Items.Add(new ComboBoxItem() { Content = item});
            }
            Ip.SelectedIndex = 1;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                ServerAccepting connecting = new ServerAccepting((string)(Ip.SelectionBoxItem), Port.Text);
                WPFUI.StartUpWindows.Windows._changePage(connecting);
            }
            catch (Exception)
            {
                MessageBox.Show("اطلاعات وارد شده مشکلی دارد ، لطفاً اطلاعات را تصحیح کنید.","خطا",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ApplicationType applicationType = new ApplicationType();
            WPFUI.StartUpWindows.Windows._changePage(applicationType);
        }
    }
}
