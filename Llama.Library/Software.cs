﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Llama.Library
{
    public class Software
    {
        ManagementObjectSearcher Win32COMPUTERSYSTEM = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");

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
