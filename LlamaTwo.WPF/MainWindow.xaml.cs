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
using Llama;

namespace LlamaTwo.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        Llama.Library.User user = new Llama.Library.User();
        Llama.Library.Hardware hw = new Llama.Library.Hardware();
        public MainWindow()
        {
            InitializeComponent();

            LoadContent();
        }

        private async void LoadContent()
        {
            lblComputerName.Content = hw.ComputerName();
            lblWelcome.Content = user.TimeOfDay() + " " + await user.UserFirstName();
            loadingUserName.IsIndeterminate = false;
            loadingUserName.IsEnabled = false;
            imgUserPhoto.ImageSource = await user.UserPhoto();
            loadingPicture.IsActive = false;
            
        }

        private void ViewDetailsPage(object sender, RoutedEventArgs e)
        {
            this.Hide();
            DetailsWindow detailsWindow = new DetailsWindow();
            detailsWindow.Show();
            this.Close();
        }
        
        private void RefreshCurrentPage(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ViewAboutPage(object sender, RoutedEventArgs e)
        {
            this.Hide();
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
            this.Close();
        }

        private void CopyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetText(hw.ComputerName());
        }
    }
}
