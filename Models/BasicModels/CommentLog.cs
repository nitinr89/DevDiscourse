using System;
using System.Net.NetworkInformation;
using System.Web;

namespace Devdiscourse.Models.BasicModels
{
    public class CommentLog : BaseClass
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
        public string ItemId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ItemType { get; set; }
        public string ActivityUrl { get; set; }
        public string MacAddress { get; set; }
        public string IPAddress { get; set; }
        public DateTime ActivityDate { get; set; }
        public bool IsRead { get; set; }

        public CommentLog()
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
        }
    }
}