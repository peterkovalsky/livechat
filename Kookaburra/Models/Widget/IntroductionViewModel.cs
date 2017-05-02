using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Models.Widget
{
    public class IntroductionViewModel
    {
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }

        [EmailAddress]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required]
        [JsonProperty("accountKey")]
        public string AccountKey { get; set; }

        [JsonProperty("url")]
        public string PageUrl { get; set; }
    }
}