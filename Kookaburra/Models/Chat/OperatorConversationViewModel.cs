using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Chat
{
    public class OperatorConversationViewModel
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("visitorName")]
        public string VisitorName { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("currentUrl")]
        public string CurrentUrl { get; set; }

        [JsonProperty("startTime")]
        public double StartTime { get; set; }

        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }

        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }

        [JsonProperty("messages")]
        public List<MessageViewModel> Messages { get; set; }
    }
}