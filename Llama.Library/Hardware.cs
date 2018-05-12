using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Llama.Library
{
    public class Hardware
    {
        ManagementObjectSearcher Win32BIOS = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
        ManagementObjectSearcher Win32COMPUTERSYSTEM = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
        ManagementObjectSearcher Win32OPERATINGSYSTEM = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        ManagementObjectSearcher Win32PROCESSOR = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
        ManagementObjectSearcher WarrantyInfo = new ManagementObjectSearcher("ROOT\\CMCO", "SELECT * FROM WarrantyInfo"); //Enter the correct WMI information

        public async Task<string[]> GetHardwareObject()
        {
            try
            {
                string[] collectedData = new string[9];
                collectedData.SetValue(await Task.Run(() => Model()), 0);
                collectedData.SetValue(await Task.Run(() => Manufacturer()), 1);
                collectedData.SetValue(await Task.Run(() => SystemFirmwareVersion()), 2);
                collectedData.SetValue(await Task.Run(() => SerialNumber()), 3);
                collectedData.SetValue(await Task.Run(() => DaysUntilWarrantyExpiration()), 4);
                collectedData.SetValue(await Task.Run(() => Processor()), 5);
                collectedData.SetValue(await Task.Run(() => Memory()), 6);
                collectedData.SetValue(ComputerName(), 7);
                collectedData.SetValue(await Task.Run(() => UpTime()), 8);


                return collectedData;
                //return user.Properties["givenName"].Value, user.Properties["sn"].Value;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public string ComputerName()
        {
            try
            {
                return System.Environment.MachineName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return "Unknown";
            }
            
        }

        public async Task<string> SerialNumber()
        {
            foreach (ManagementObject wmi in Win32BIOS.Get())
            {
                try
                {
                    return await Task.Run(() => wmi.GetPropertyValue("SerialNumber").ToString());
                }
                catch {
                    return "Unknown";
                }
            }

            return "Unknown";
        }

        public async Task<string> Model()
        {
            foreach (ManagementObject wmi in Win32COMPUTERSYSTEM.Get())
            {
                try
                {
                    return await Task.Run(() => wmi.GetPropertyValue("Model").ToString());
                }
                catch
                {
                    return "Unknown";
                }
            }

            return "Unknown";
        }

        public async Task<string> Manufacturer()
        {
            foreach (ManagementObject wmi in Win32COMPUTERSYSTEM.Get())
            {
                try
                {
                    return await Task.Run(() => wmi.GetPropertyValue("Manufacturer").ToString());
                }
                catch
                {
                    return "Unknown";
                }
            }

            return "Unknown";
        }

        public async Task<string> SystemFirmwareVersion()
        {
            foreach (ManagementObject wmi in Win32BIOS.Get())
            {
                try
                {
                    return await Task.Run(() => wmi.GetPropertyValue("SMBIOSBIOSVersion").ToString());
                }
                catch
                {
                    return "Unknown";
                }
            }

            return "Unknown";
        }

        public async Task<string> SystemType()
        {
            foreach (ManagementObject wmi in Win32COMPUTERSYSTEM.Get())
            {
                try
                {
                    return await Task.Run(() => wmi.GetPropertyValue("SystemType").ToString());
                }
                catch
                {
                    return "Unknown";
                }
            }

            return "Unknown";
        }

        public async Task<string> Chassis()
        {
            foreach (ManagementObject wmi in Win32COMPUTERSYSTEM.Get())
            {
                try
                {
                    return await Task.Run(() => wmi.GetPropertyValue("ChassisSKUNumber").ToString());
                }
                catch
                {
                    return "Unknown";
                }
            }

            return "Unknown";
        }
        public async Task<DateTime> WarrantyExpiration()
        {
            foreach (ManagementObject wmi in WarrantyInfo.Get())
            {
                try
                {
                    return await Task.Run(() => Convert.ToDateTime(wmi.GetPropertyValue("EndDate")));
                }
                catch
                {
                    return DateTime.MinValue;
                }
            }

            return DateTime.MinValue;
        }

        public async Task<string> DaysUntilWarrantyExpiration()
        {
            DateTime dtExpire = await WarrantyExpiration();
            DateTime dtNow = DateTime.Now.Date;
            TimeSpan ts = dtExpire.Subtract(dtNow);
            if (ts.Days <= 0)
            {
                return ":(";
            }
            else
            {
                return ts.Days.ToString();
            }
        }

        public async Task<string> Processor()
        {
            foreach (ManagementObject wmi in Win32PROCESSOR.Get())
            {
                try
                {
                    return await Task.Run(() => wmi.GetPropertyValue("Name").ToString());
                }
                catch
                {
                    return "Unknown";
                }
            }

            return "Unknown";
        }

        public async Task<string> Memory()
        {
            foreach (ManagementObject wmi in Win32COMPUTERSYSTEM.Get())
            {
                try
                {
                    long rawMemory = 0;
                    await Task.Run(() => rawMemory = Convert.ToInt64(wmi.GetPropertyValue("TotalPhysicalMemory").ToString()));
                    rawMemory = rawMemory / 1024 / 1024 / 1000;
                    // Todo: better calculation here
                    return rawMemory.ToString();
                }
                catch
                {
                    return "Unknown";
                }
            }

            return "Unknown";
        }
        
        public async Task<bool> DomainMember()
        {
            foreach (ManagementObject wmi in Win32COMPUTERSYSTEM.Get())
            {
                try
                {

                    string isDomainJoined = await Task.Run(() => wmi.GetPropertyValue("PartOfDomain").ToString());
                    if (isDomainJoined == "True") { return true; }
                    else { return false; }
                }
                catch
                {
                    return true;
                }
            }

            return true;
        }

        public async Task<string> UpTime()
        {
            foreach (ManagementObject wmi in Win32OPERATINGSYSTEM.Get())
            {
                try
                {
                    string uptime = "";
                    DateTime lastBoot = await Task.Run(() => ManagementDateTimeConverter.ToDateTime(wmi.GetPropertyValue("LastBootUpTime").ToString()));
                    //uptime = await Task.Run(() => (DateTime.Now.ToUniversalTime() - lastBoot.ToUniversalTime()).ToString());
                    uptime = "Up for " + (DateTime.Now.ToUniversalTime() - lastBoot.ToUniversalTime()).ToString() + " days.";
                    return uptime;
                }
                catch
                {
                    return "Unknown";
                }
            }

            return "Unknown";

        }

        public string IPAddress()
        {
            string strIP = null;
            try
            {
                System.Net.IPHostEntry IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());

                // IPAddress class contains the address of a computer on an IP network. 
                foreach (System.Net.IPAddress ipAddress in IPHostEntry.AddressList)
                {
                    // InterNetwork indicates that an IP version 4 address is expected 
                    // when a Socket connects to an endpoint
                    if (ipAddress.AddressFamily.ToString() == "InterNetwork")
                    {
                        strIP = ipAddress.ToString();
                    }
                }
                return strIP;
            }
            catch
            {
                return "Unknown";
            }
        }

    }
}
