using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.ServiceProcess;

namespace Llama.Library
{
    public class Software
    {
        ManagementObjectSearcher Win32COMPUTERSYSTEM = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
        ManagementObjectSearcher Win32OPERATINGSYSTEM = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        ManagementObjectSearcher Win32ANTIVIRUSPRODUCT = new ManagementObjectSearcher("ROOT\\SecurityCenter2", "SELECT * FROM AntiVirusProduct");
        ManagementObjectSearcher Win32RELIABILITYSTABILITYMETRICS = new ManagementObjectSearcher("SELECT * FROM  Win32_ReliabilityStabilityMetrics");
        ManagementObjectSearcher Win32PNPENTITY = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity");

        public string OperatingSystemName()
        {
            try
            {
                return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", string.Empty).ToString();
            }
            catch
            {
                return "Unknown";
            }
        }

        public string OperatingSystemBuild()
        {
            try
            {
                return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "BuildLab", string.Empty).ToString();
            }
            catch
            {
                return "Unknown";
            }

        }

        public string OperatingSystemReleaseId()
        {
            try
            {
                return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", string.Empty).ToString();
            }
            catch
            {
                return "N/A";
            }

        }

        public async Task<string> ComputerDomain()
        {
            try
            {
                foreach (ManagementObject wmi in Win32COMPUTERSYSTEM.Get())
                {
                    try
                    {
                        return await Task.Run(() => wmi.GetPropertyValue("Domain").ToString());
                    }
                    catch
                    {
                        return "Unknown";
                    }
                }

                return "Unknown";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "Unknown";
            }
        }

        public async Task<string> InstalledThreatProtectionProduct()
        {
            try
            {
                foreach (ManagementObject wmi in Win32ANTIVIRUSPRODUCT.Get())
                {
                    try
                    {
                        return await Task.Run(() => wmi.GetPropertyValue("displayName").ToString());
                    }
                    catch
                    {
                        return "Not Running";
                    }
                }

                return "Unknown";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "Unknown";
            }
        }

        public async Task<string> ThreatProtectionStatus()
        {
            uint threatProtectionStatus = 0;
            string hexThreatProtectionStatus = null;
            string output = null;
            try
            {
                foreach (ManagementObject wmi in Win32ANTIVIRUSPRODUCT.Get())
                {
                    try
                    {
                        threatProtectionStatus = await Task.Run(() => Convert.ToUInt32(wmi.GetPropertyValue("productState").ToString()));
                        hexThreatProtectionStatus = threatProtectionStatus.ToString("X");
                        switch (hexThreatProtectionStatus.Substring(1, 2))
                        {
                            case "10":
                                output = "Not Enabled";
                                Console.WriteLine("Security product is not enabled");
                                break;
                            case "11":
                                output = "Enabled";
                                Console.WriteLine("Security product is enabled");
                                break;
                        }
                        output = output + " and ";

                        switch (hexThreatProtectionStatus.Substring(3, 2))
                        {
                            case "00":
                                output = output + "up to date";
                                Console.WriteLine("Security product is up to date");
                                break;
                            case "10":
                                output = output + "not up to date";
                                Console.WriteLine("Security product is not up to date");
                                break;
                        }
                        return output;
                    }
                    catch
                    {
                        return "Unknown";
                    }
                }

                return "Unknown";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "Unknown";
            }
        }

        public async Task<string> FirewallStatus()
        {
            return "Not Implemented";
        }

        public async Task<string> GetDriverFaults()
        {
            int driverFaultCount = 0;
            foreach (ManagementObject wmi in Win32PNPENTITY.Get())
            {
                try
                {
                    if ((!(await Task.Run(() => wmi.GetPropertyValue("ConfigManagerErrorCode").ToString()) == "0")) && (!(await Task.Run(() => wmi.GetPropertyValue("ConfigManagerErrorCode").ToString()) == "22")) && (!(await Task.Run(() => wmi.GetPropertyValue("Name").ToString().Contains("PS/2")))))
                    {
                        driverFaultCount++;
                    }
                }
                catch
                {
                    return "Unknown";
                }
            }
            if (driverFaultCount == 0)
            {
                return "OK";
            }
            else
            {
                return driverFaultCount.ToString() + " faulty drivers";
            }
        }

        public async Task<string> CheckCcmexecService()
        {
            try
            {
                using (ServiceController sc = new ServiceController("ccmexec"))
                {
                    return "OK";
                }
            }
            catch
            {
                return "Service is not running";
            }
        }

        public async Task<string> UACStatus()
        {
            try
            {
                string uac = await Task.Run(() => Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "EnableLUA", string.Empty).ToString());
                if (uac == "1") { return "Enabled"; }
                else { return "Disabled"; }
            }
            catch
            {
                return "Unknown";
            }
        }

        public async Task<string> BootMode()
        {
            try
            {
                string bootMode = await Task.Run(() => Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecureBoot\State", "UEFISecureBootEnabled", string.Empty).ToString());
                if (bootMode == "1") { return "UEFI"; }
                else { return "Legacy BIOS"; }
            }
            catch
            {
                return "Legacy BIOS";
            }
        }

        public async Task<string> SecureBootStatus()
        {
            try
            {
                string secureBootMode = await Task.Run(() => Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecureBoot\State", "UEFISecureBootEnabled", string.Empty).ToString());
                if (secureBootMode == "1") { return "Enabled"; }
                else { return "Disabled"; }
            }
            catch
            {
                return "Disabled";
            }
        }

        public async Task<string> StabilityIndexScore()
        {
            foreach (ManagementObject wmi in Win32RELIABILITYSTABILITYMETRICS.Get())
            {
                try
                {
                    return await Task.Run(() => wmi.GetPropertyValue("SystemStabilityIndex").ToString());
                }
                catch
                {
                    return "?";
                }
            }

            return "?";
        }

        public async Task<string> OperatingSystemBitness()
        {
            foreach (ManagementObject wmi in Win32OPERATINGSYSTEM.Get())
            {
                try
                {
                    return await Task.Run(() => "(" + wmi.GetPropertyValue("OSArchitecture").ToString() + ")");
                }
                catch
                {
                    return "";
                }
            }

            return "";

        }

        public bool IsRebootPending()
        {
            bool wellIsIt = false;
            try
            {
                if (!(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing", "RebootPending", string.Empty).ToString() == null)) { wellIsIt = true; }
            }
            catch
            {
                Console.WriteLine("CBS is not in a reboot pending state");
            }

            try
            {
                if (!(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update", "RebootRequired", string.Empty).ToString() == null)) { wellIsIt = true; }
            }
            catch
            {
                Console.WriteLine("Windows Update is not in a reboot pending state");
            }

            try
            {
                if (!(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SessionManager", "PendingFileRenameOperations", string.Empty).ToString() == null)) { wellIsIt = true; }
            }
            catch
            {
                Console.WriteLine("PendingFileRenameOperations is not in a reboot pending state");
            }

            // Add pending reboot check from clientsdk:CCM_ClientUtilities

            return wellIsIt;
        }
    }
}
