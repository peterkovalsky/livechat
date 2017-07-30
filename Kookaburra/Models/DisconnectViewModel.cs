using Newtonsoft.Json;

namespace Kookaburra.Models
{
    public class DisconnectViewModel
    {
        [JsonProperty("visitorSessionId")]
        public string VisitorSessionId { get; set; }

        [JsonProperty("time")]
        public double TimeStamp { get; set; }

        [JsonProperty("disconnectedBy")]
        public string DisconnectedBy { get; set; }
    }
}