using Kookaburra.Domain.Common;
using Kookaburra.Domain.Integration;
using Kookaburra.Domain.Integration.Model;
using System.Threading.Tasks;

namespace Kookaburra.Integration.freegeoip
{
    public class FreegeoipLocator : IGeoLocator
    {
        public VisitorLocation GetLocation(string ip)
        {
            if (Helper.ValidateIPv4(ip))
            {
                var geolocator = new Geolocator();
                var location = geolocator.GetLocation(ip);

                if (location != null)
                {
                    return new VisitorLocation
                    {
                        Country = location.CountryName,
                        Region = location.RegionName,
                        City = location.City,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    };
                }
            }

            return null;
        }

        public async Task<VisitorLocation> GetLocationAsync(string ip)
        {
            if (Helper.ValidateIPv4(ip))
            {
                var geolocator = new Geolocator();
                var location = await geolocator.GetLocationAsync(ip);

                if (location != null)
                {
                    return new VisitorLocation
                    {
                        Country = location.CountryName,
                        Region = location.RegionName,
                        City = location.City,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude
                    };
                }
            }

            return null;
        }    
    }
}