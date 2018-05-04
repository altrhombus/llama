using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Llama.Library
{
    public class Software
    {

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
    }
}
