using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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
using MahApps.Metro.Controls.Dialogs;

namespace LlamaTwo.WPF
{
    /// <summary>
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow
    {
        Llama.Library.User user = new Llama.Library.User();
        Llama.Library.Hardware hw = new Llama.Library.Hardware();
        Llama.Library.Software sw = new Llama.Library.Software();
        public DetailsWindow()
        {
            InitializeComponent();

            LoadYouContent();
            LoadPCContent();
            LoadHealthContent();
        }

        private async void LoadYouContent()
        {
            try
            {
                string[] populateTheFields = new string[11];
                string unformattedManager;
                int pwExpiration;

                pwExpiration = await user.DaysUntilPasswordExpiration();

                if (pwExpiration == -180)
                {
                    lblPwExpiration.Content = "?";
                }
                else
                {
                    lblPwExpiration.Content = pwExpiration.ToString();
                }

                imgUserPhoto.ImageSource = await user.UserPhoto();

                populateTheFields = await user.GetActiveDirectoryObject();

                lblUserFullName.Content = populateTheFields[0] + " " + populateTheFields[1];
                lblUserTitle.Text = populateTheFields[2];
                lblTelephone.Content = populateTheFields[3];
                lblMobilePhone.Content = populateTheFields[4];
                lblFaxPhone.Content = populateTheFields[5];
                lblEmailAddress.Content = populateTheFields[6];
                lblSIPAddress.Content = populateTheFields[7];
                lblPhysicalAddress.Text = populateTheFields[8];
                lblDept.Content = populateTheFields[9];
                unformattedManager = populateTheFields[10];
                unformattedManager = unformattedManager.Replace("CN=", string.Empty);
                unformattedManager = unformattedManager.Replace("\\", string.Empty);
                int index = unformattedManager.IndexOf(",OU");
                lblManager.Content = unformattedManager.Substring(0, index);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            loadingYouPage.IsActive = false;
        }

        private async void LoadPCContent()
        {
            try
            {
                string[] populateTheFields = new string[9];

                populateTheFields = await hw.GetHardwareObject();

                lblModel.Content = populateTheFields[0];
                lblManufacturer.Content = populateTheFields[1];
                lblFwVer.Content = populateTheFields[2];
                lblSysType.Content = populateTheFields[3];
                lblHwWarExpiration.Content = populateTheFields[4];
                lblCPU.Content = populateTheFields[5];
                lblMemory.Content = populateTheFields[6] + "GB";
                lblComputerName.Content = populateTheFields[7];
                lblUptime.Text = populateTheFields[8];

                lblOSName.Content = sw.OperatingSystemName();
                lblOSBuild.Content = sw.OperatingSystemBuild();
                lblOSRelease.Content = sw.OperatingSystemReleaseId();
                chkIsRebootPending.IsChecked = sw.IsRebootPending();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            loadingYourComputerPage.IsActive = false;
        }

        private async void LoadHealthContent()
        {
            try
            {
                lblStabilityIndexScore.Content = await sw.StabilityIndexScore();
                lblAVStatus.Content = await sw.AntiVirusStatus();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            loadingHealthPage.IsActive = false;
        }

        private void ViewUndetailsPage(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void RefreshCurrentPage(object sender, RoutedEventArgs e)
        {
            DetailsWindow detailsWindow = new DetailsWindow();
            detailsWindow.Show();
            this.Close();
        }

        private void ViewAboutPage(object sender, RoutedEventArgs e)
        {
            this.Hide();
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Show();
            this.Close();
        }

        private void btnHwInvCycle_Click(object sender, RoutedEventArgs e)
        {
            TriggerSchedule(hw.ComputerName(), "00000000-0000-0000-0000-000000000001");
        }

        void TriggerSchedule(string computerName, string ScheduleID)
        {
            try  // Get the client's SMS_Client class.
            {
                ManagementScope scp = new ManagementScope(string.Format(@"\\{0}\root\ccm", "."));
                ManagementClass cls = new ManagementClass(scp.Path.Path, "sms_client", null);
                ManagementBaseObject inParams;

                // Set up the machine policy & evaluation string as input parameter for TriggerSchedule.
                inParams = cls.GetMethodParameters("TriggerSchedule");
                inParams["sScheduleID"] = "{" + ScheduleID + "}";

                // Trigger the machine policy and evaluation cycle.
                ManagementBaseObject outMPParams = cls.InvokeMethod("TriggerSchedule", inParams, null);
                lblConfigMgrStatus.Content = "Initiated!";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                lblConfigMgrStatus.Content = ex.Message;
            }
        }

        private async void btnReportUserInfo_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("Update Incorrect Information", "To update incorrect information about your account, please submit a request at the Service Desk.", MessageDialogStyle.Affirmative);
        }

        private async void btnChangePwd_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("Change Password", "To change your password, press CTRL + ALT + DEL and select 'Change a password.'", MessageDialogStyle.Affirmative);
        }

        private void btnHwInvCycle_Click(object sender, MouseButtonEventArgs e)
        {
            TriggerSchedule(hw.ComputerName(), "{00000000-0000-0000-0000-000000000022}");
        }
        
    }
}
