using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Web;

namespace Devdiscourse.Models.BasicModels
{
    public class ActivityLog : BaseClass
    {
        public override Guid Id
        {
            get
            {
                return base.Id;
            }

            set
            {
                base.Id = value;
            }
        }
        public string LogTitle { get; set; }
        public string LogDescription { get; set; }
        public string ActivityUserId { get; set; }
        public string CreatorId { get; set; }
        public string Activity { get; set; }
        public string ActivityUrl { get; set; }
        public string MacAddress { get; set; }
        public string IPAddress { get; set; }
        public DateTime ActivityDate { get; set; }
        public bool IsRead { get; set; }
        [ForeignKey("ActivityUserId")]
        public virtual ApplicationUser ApplicationUsers { get; set; }

        public ActivityLog()
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
            string VisitorIp = "";
            //HttpContext context = System.Web.HttpContext.Current;
            //string ipAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            //if (!string.IsNullOrEmpty(ipAddress))
            //{
            //    string[] addresses = ipAddress.Split(':');
            //    if (addresses.Length != 0)
            //    {
            //        VisitorIp = addresses[0];
            //    }
            //}
            return VisitorIp;
            throw new Exception("IP Address Not Found!");
            //var host = Dns.GetHostEntry(Dns.GetHostName());
            //foreach (var ip in host.AddressList)
            //{
            //    if (ip.AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        return ip.ToString();
            //    }
            //}
            //throw new Exception("Local IP Address Not Found!");
        }
    }
}