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
    /// Interaction logic for ApplicationType.xaml
    /// </summary>
    public partial class ApplicationType : UserControl
    {
        public ApplicationType()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ServerInit serverInit = new ServerInit();
            WPFUI.StartUpWindows.Windows._changePage(serverInit);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ClientInit clientInit = new ClientInit();
            WPFUI.StartUpWindows.Windows._changePage(clientInit);
        }
    }
}
