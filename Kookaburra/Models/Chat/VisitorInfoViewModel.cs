using Newtonsoft.Json;

namespace Kookaburra.Models.Chat
{
    public class VisitorInfoViewModel
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("currentUrl")]
        public string CurrentUrl { get; set; }

        [JsonProperty("time")]
        public double Time { get; set; }
    }
}