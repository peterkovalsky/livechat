using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kookaburra.Tests
{
    [TestClass]
    public class WebTests
    {
        [TestMethod]
        public void TestIp()
        {         
            Assert.AreEqual("14.203.211.244", GetIp("14.203.211.244:61827"));
            Assert.AreEqual("14.203.211.244", GetIp("14.203.211.244:61827,15.203.211.244:55898,16.203.211.244:55898"));
            Assert.AreEqual("14.203.211.244", GetIp("14.203.211.244"));
            Assert.AreEqual("14.203.211.244", GetIp("14.203.211.244,15.203.211.244"));
        }

        private string GetIp(string rawIp)
        {
            string ipAddress = rawIp;

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    ipAddress = addresses[0];
                }
            }           

            // contains port so we need to remove it
            if (!string.IsNullOrWhiteSpace(ipAddress) && ipAddress.Contains(":"))
            {
                ipAddress = ipAddress.Substring(0, ipAddress.IndexOf(":"));
            }

            return ipAddress;
        }
    }
}