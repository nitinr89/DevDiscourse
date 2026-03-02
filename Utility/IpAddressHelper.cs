namespace Devdiscourse.Utility
{
    public class IpAddressHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IpAddressHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetVisitorIp()
        {
            var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            string visitorIp = "";

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(':');
                if (addresses.Length != 0)
                {
                    visitorIp = addresses[0];
                }
            }

            return visitorIp;
        }
    }

}
