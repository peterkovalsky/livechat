using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Kookaburra.Integration.freegeoip
{
    public class Geolocator
    {
        //{"ip":"205.144.171.97",
        //"country_code":"US",
        //"country_name":"United States",
        //"region_code":"CA",
        //"region_name":"California",
        //"city":"Studio City",
        //"zip_code":"91604",
        //"time_zone":"America/Los_Angeles",
        //"latitude":34.1379,
        //"longitude":-118.3919,
        //"metro_code":803}

        private readonly HttpClient _client;

        public Geolocator()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://freegeoip.net/");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public Location GetLocation(string ipOrHost)
        {
            Location location = null;

            var response = _client.GetAsync("json/" + ipOrHost);
            if (response.Result.IsSuccessStatusCode)
            {
                location = response.Result.Content.ReadAsAsync<Location>().Result;
            }

            return location;
        }

        public async Task<Location> GetLocationAsync(string ipOrHost)
        {
            Location location = null;     

            HttpResponseMessage response = await _client.GetAsync("json/" + ipOrHost);
            if (response.IsSuccessStatusCode)
            {
                location = await response.Content.ReadAsAsync<Location>();
            }

            return location;
        }
    }
}