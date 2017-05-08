using Newtonsoft.Json;

namespace Kookaburra.Models.Widget
{
    public class StartChatViewModel
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("operatorAvailable")]
        public bool OperatorAvailable { get; set; }

        [JsonProperty("operatorName")]
        public string OperatorName { get; set; }
    }
}