using Newtonsoft.Json;
using System;

namespace Kookaburra.Models.Offline
{
    public class LeftMessageViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string VisitorName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("isRead")]
        public bool IsRead { get; set; }
    }
}