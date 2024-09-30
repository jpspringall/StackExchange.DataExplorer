using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

namespace StackExchange.DataExplorer.Helpers
{
    public static class IpUtilities
    {
        private static readonly Regex _ipAddress = new Regex(@"\b([0-9]{1,3}\.){3}[0-9]{1,3}$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static bool IsPrivateIP(string s)
        {
            return s.StartsWith("192.168.") || s.StartsWith("10.") || s.StartsWith("127.0.0.");
        }

        /// <summary>
        /// retrieves the IP address of the current request -- handles proxies and private networks
        /// </summary>
        public static string GetRemoteIP(NameValueCollection ServerVariables)
        {
            var ip = ServerVariables["REMOTE_ADDR"]; // could be a proxy -- beware
            var ipForwarded = ServerVariables["HTTP_X_FORWARDED_FOR"];

            // check if we were forwarded from a proxy
            if (ipForwarded.HasValue())
            {
                ipForwarded = _ipAddress.Match(ipForwarded).Value;
                if (ipForwarded.HasValue() && !IsPrivateIP(ipForwarded))
                    ip = ipForwarded;
            }

            return ip.HasValue() ? ip : "X.X.X.X";
        }


        public static string GetRemoteIP()
        {
            NameValueCollection ServerVaraibles;

            // This is a nasty hack so we don't crash the non-request test cases
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                ServerVaraibles = HttpContext.Current.Request.ServerVariables;
            }
            else
            {
                ServerVaraibles = new NameValueCollection();
            }

            return GetRemoteIP(ServerVaraibles);
        }
    }
}
