using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Llama.Library
{
    public class User
    {
        public string CurrentDomain()
        {
            return System.Environment.UserDomainName;
        }
        public async Task<string> UserFirstName()
        {
            try
            {
                UserPrincipal userPrincipal = await Task.Run(() => UserPrincipal.Current);
                return userPrincipal.GivenName;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public string UserName()
        {
            return System.Environment.UserName;
        }

        public string DisplayName()
        {
            return System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName;
        }

        public async Task<DateTime> PasswordExpirationDate()
        {
            try
            {
                var userEntry = await Task.Run(() => new DirectoryEntry("WinNT://" + CurrentDomain() + "/" + UserName() + ",user"));
                return await Task.Run(() => (DateTime)userEntry.InvokeGet("PasswordExpirationDate"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return DateTime.MinValue;
            }
        }

        public async Task<int> DaysUntilPasswordExpiration()
        {
            DateTime dtExpire = await PasswordExpirationDate();
            DateTime dtNow = DateTime.Now.Date;
            TimeSpan ts = dtExpire.Subtract(dtNow);
            if (ts.Days < 0)
            {
                return -180;
            }
            else
            {
                return ts.Days;
            }
            
        }
        public async Task<string[]> GetActiveDirectoryObject()
        {
            try
            {
                DirectorySearcher dsSearcher = new DirectorySearcher();
                dsSearcher.Asynchronous = true;
                dsSearcher.Filter = "(&(objectClass=user) (sAMAccountName=" + UserName() + "))";
                dsSearcher.PropertiesToLoad.Add("givenName");
                dsSearcher.PropertiesToLoad.Add("sn");
                dsSearcher.PropertiesToLoad.Add("title");
                dsSearcher.PropertiesToLoad.Add("telephoneNumber");
                dsSearcher.PropertiesToLoad.Add("mobile");
                dsSearcher.PropertiesToLoad.Add("facsimileTelephoneNumber");
                dsSearcher.PropertiesToLoad.Add("mail");
                dsSearcher.PropertiesToLoad.Add("msRTCSIP-PrimaryUserAddress");
                dsSearcher.PropertiesToLoad.Add("physicalDeliveryOfficeName");
                dsSearcher.PropertiesToLoad.Add("department");
                dsSearcher.PropertiesToLoad.Add("manager");
                //dsSearcher.PropertiesToLoad.Add("");
                SearchResult result = await Task.Run(() => dsSearcher.FindOne());
                DirectoryEntry user = new DirectoryEntry(result.Path);
                string[] collectedData = new string[11];
                collectedData.SetValue(user.Properties["givenName"].Value, 0);
                collectedData.SetValue(user.Properties["sn"].Value, 1);
                collectedData.SetValue(user.Properties["title"].Value, 2);
                collectedData.SetValue(user.Properties["telephoneNumber"].Value, 3);
                collectedData.SetValue(user.Properties["mobile"].Value, 4);
                collectedData.SetValue(user.Properties["facsimileTelephoneNumber"].Value, 5);
                collectedData.SetValue(user.Properties["mail"].Value, 6);
                collectedData.SetValue(user.Properties["msRTCSIP-PrimaryUserAddress"].Value, 7);
                collectedData.SetValue(user.Properties["physicalDeliveryOfficeName"].Value, 8);
                collectedData.SetValue(user.Properties["department"].Value, 9);
                collectedData.SetValue(user.Properties["manager"].Value, 10);

                return collectedData;
                //return user.Properties["givenName"].Value, user.Properties["sn"].Value;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
        public async Task<BitmapImage> UserPhoto()
        {
            try
            {
                DirectorySearcher dsSearcher = new DirectorySearcher();
                dsSearcher.Asynchronous = true;
                dsSearcher.Filter = "(&(objectClass=user) (sAMAccountName=" + UserName() + "))";
                SearchResult result = await Task.Run(() => dsSearcher.FindOne());
                DirectoryEntry user = await Task.Run(() => new DirectoryEntry(result.Path));
                byte[] data = user.Properties["thumbnailPhoto"].Value as byte[];
                var ms = new MemoryStream(data);
                var imageSource = new BitmapImage();
                imageSource.BeginInit();
                imageSource.StreamSource = ms;
                imageSource.EndInit();
                return imageSource;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public string TimeOfDay()
        {
            if (DateTime.Now.Hour < 12) { return "Good morning,"; }
            else if (DateTime.Now.Hour > 12 && DateTime.Now.Hour < 17) { return "Good afternoon,"; }
            else if (DateTime.Now.Hour > 17) { return "Good evening,"; }
            else return "Greetings,";
        }


    }
}