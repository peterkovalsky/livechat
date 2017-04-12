using Newtonsoft.Json;
using System;

namespace Kookaburra.Models.History
{
    public class ConversationItemViewModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("visitorName")]
        public string VisitorName { get; set; }

        [JsonProperty("operatorName")]
        public string OperatorName { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("totalMessages")]
        public int TotalMessages { get; set; }
    }
}