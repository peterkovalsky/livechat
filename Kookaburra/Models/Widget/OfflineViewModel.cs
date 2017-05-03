using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Models.Widget
{
    public class OfflineViewModel
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required]
        [JsonProperty("message")]
        public string Message { get; set; }

        [Required]
        [JsonProperty("accountKey")]
        public string AccountKey { get; set; }
    }
}