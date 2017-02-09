using Kookaburra.Integration.freegeoip;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kookaburra.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void TestGeolocation()
        {
            var freegeoip = new Geolocator();

            var location = freegeoip.GetLocation("rio-matras.com");

            Assert.AreEqual("205.144.171.97", location.Ip);
            Assert.AreEqual("US", location.CountryCode);
            Assert.AreEqual("United States", location.CountryName);
            Assert.AreEqual("CA", location.RegionCode);
            Assert.AreEqual("California", location.RegionName);
            Assert.AreEqual("Studio City", location.City);
            Assert.AreEqual("91604", location.Zip);
            Assert.AreEqual("America/Los_Angeles", location.TimeZone);
            Assert.AreEqual(34.1379m, location.Latitude);
            Assert.AreEqual(-118.3919m, location.Longitude);
            Assert.AreEqual("803", location.MetroCode);
        }

        [TestMethod]
        public void TestGeolocationAsync()
        {
            var freegeoip = new Geolocator();

            var locationTask = freegeoip.GetLocationAsync("::1");
            var location = locationTask.Result;

            Assert.AreEqual("205.144.171.97", location.Ip);
            Assert.AreEqual("US", location.CountryCode);
            Assert.AreEqual("United States", location.CountryName);
            Assert.AreEqual("CA", location.RegionCode);
            Assert.AreEqual("California", location.RegionName);
            Assert.AreEqual("Studio City", location.City);
            Assert.AreEqual("91604", location.Zip);
            Assert.AreEqual("America/Los_Angeles", location.TimeZone);
            Assert.AreEqual(34.1379m, location.Latitude);
            Assert.AreEqual(-118.3919m, location.Longitude);
            Assert.AreEqual("803", location.MetroCode);
        }
    }
}