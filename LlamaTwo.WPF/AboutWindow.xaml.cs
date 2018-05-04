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
using System.Deployment.Application;

namespace LlamaTwo.WPF
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.lblVersion.Content = version.Major.ToString() + "." + version.Minor.ToString() + "." + version.Build.ToString() + "." + version.Revision.ToString();
        }
        private void ViewDetailsPage(object sender, RoutedEventArgs e)
        {
            this.Hide();
            DetailsWindow detailsWindow = new DetailsWindow();
            detailsWindow.Show();
            this.Close();
        }

        private void ViewUndetailsPage(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
