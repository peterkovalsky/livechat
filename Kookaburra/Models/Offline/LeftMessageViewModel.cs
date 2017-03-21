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
        public DateTime Time { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("isRead")]
        public bool IsRead { get; set; }
    }
}