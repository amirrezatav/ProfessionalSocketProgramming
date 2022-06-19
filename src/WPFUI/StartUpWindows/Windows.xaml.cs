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
using System.Windows.Shapes;
using WPFUI.StartUpWindows.Pages;

namespace WPFUI.StartUpWindows
{
    public delegate void ChangePage(UserControl page);
    public delegate void CloseWindows();
    /// <summary>
    /// Interaction logic for Windows.xaml
    /// </summary>
    public partial class Windows : Window
    {
        public static  ChangePage _changePage;
        public static CloseWindows _CloseWindows;
        public Windows()
        {
            InitializeComponent();
            ApplicationType applicationType = new ApplicationType();
            ChangeGridPage(applicationType);
            _changePage += ChangeGridPage;
            _CloseWindows += Close_Windows;
        }

        private void Close_Windows()
        {
            this.Close();
        }

        public void ChangeGridPage(UserControl page)
        {
            Content.Children.Clear();
            Content.Children.Add(page);
        }
    }
}
