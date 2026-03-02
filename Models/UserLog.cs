using System.Net.NetworkInformation;

namespace Devdiscourse.Models
{
    public class UserLog : BaseClass
    {
        public override Guid Id { get; set; }
        public string LogTitle { get; set; }
        public string ActivityUrl { get; set; }
        public string MacAddress { get; set; }
        public string IPAddress { get; set; }
        public UserLog()
        {
            MacAddress = GetMACAddress();
            IPAddress = GetLocalIPAddress();
        }

        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            string sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == string.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        public string GetLocalIPAddress()
        {
            string VisitorIp = "127.0.0.1"; 
            return VisitorIp;
            throw new Exception("IP Address Not Found!");
        }
    }
}