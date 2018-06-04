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
        Llama.Library.Troubleshooting tb = new Llama.Library.Troubleshooting();

        public DetailsWindow()
        {
            InitializeComponent();

            LoadYouContent();
            LoadPCContent();
            LoadNetworkContent();
            LoadHealthContent();

        }

        private async void LoadYouContent()
        {
            try
            {
                if (await hw.DomainMember() == true)
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
                else
                {
                    tabYou.Visibility = Visibility.Collapsed;
                    tabControlDetails.SelectedIndex = 1;
                }
                

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
                lblSerialNumber.Content = populateTheFields[3];
                lblHwWarExpiration.Content = populateTheFields[4];
                lblCPU.Content = populateTheFields[5];
                lblMemory.Content = populateTheFields[6] + "GB";
                lblComputerName.Content = populateTheFields[7];
                lblUptime.Text = populateTheFields[8];

                lblOSName.Content = sw.OperatingSystemName() + " " + await sw.OperatingSystemBitness();
                lblOSBuild.Content = sw.OperatingSystemBuild();
                lblOSRelease.Content = sw.OperatingSystemReleaseId();
                chkIsRebootPending.IsChecked = sw.IsRebootPending();

                if (await hw.Chassis() == "Notebook") { imgChassis.Fill = new VisualBrush() { Visual = (Visual)Resources["appbar_laptop"] }; }
                if (await hw.Chassis() == "Virtual Machine") { imgChassis.Fill = new VisualBrush() { Visual = (Visual)Resources["appbar_companioncube"] }; }


            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            loadingYourComputerPage.IsActive = false;
        }

        private async void LoadNetworkContent()
        {
            try
            {
                lblIPAddress.Content = hw.IPAddress();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private async void LoadHealthContent()
        {
            try
            {
                lblStabilityIndexScore.Content = await sw.StabilityIndexScore();
                lblThreatProduct.Content = await sw.InstalledThreatProtectionProduct();
                lblThreatStatus.Content = await sw.ThreatProtectionStatus();
                lblUACStatus.Content = await sw.UACStatus();
                lblBootMode.Content = await sw.BootMode();
                lblSecureBootStatus.Content = await sw.SecureBootStatus();
                lblMissingDrivers.Content = await sw.GetDriverFaults();
                lblCcmexecSvc.Content = await sw.CheckCcmexecService();
                lblFwStatus.Content = sw.FirewallStatus();
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

        private async void btnSIS_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "perfmon.exe";
            process.StartInfo.Arguments = "/rel";
            process.Start();
            await this.ShowMessageAsync("Stability Index Score", "The stability index assesses your system's overall stability on a scale from 1 to 10. The Reliability Monitor has been launched so you can review historical data.", MessageDialogStyle.Affirmative);
        }

        private async void lbiCcmLogs_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var ccmLogsExport = await this.ShowProgressAsync("Exporting CCM Logs", "Copying logs to a temporary location...");
            ccmLogsExport.SetIndeterminate();
            await tb.CollectCCMLogs();
            ccmLogsExport.SetMessage("Compressing log files...");
            await tb.CompressCCMLogs();
            await ccmLogsExport.CloseAsync();
            await this.ShowMessageAsync("CCM Logs Exported", "A CCMLOGS zip file has been placed on your desktop. Please submit this file to your technical team for further assistance.", MessageDialogStyle.Affirmative);
        }

        private async void btn_NeedHelp_Click(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("We're here to help!", "Contact information about the service desk could be posted here!", MessageDialogStyle.Affirmative);
        }
    }
}
