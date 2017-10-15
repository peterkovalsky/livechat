using System;
using System.Web;

namespace Kookaburra.Common
{
    public static class WebHelper
    {
        public static string GetIPAddress()
        {
            HttpContext context = HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    ipAddress = addresses[0];
                }
            }
            else
            {
                ipAddress = context.Request.ServerVariables["REMOTE_ADDR"];
            }

            // contains port so we need to remove it
            if (!string.IsNullOrWhiteSpace(ipAddress) && ipAddress.Contains(":"))
            {
                ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(":"));
            }
            
            return ipAddress;
        }

        public static string Host
        {
            get
            {
                HttpContext context = HttpContext.Current;

                return context.Request.Url.Scheme + Uri.SchemeDelimiter + context.Request.Url.Host + 
                    (context.Request.Url.Host == "localhost" ? ":" + context.Request.Url.Port.ToString() : "");
            }
        }
    }
}