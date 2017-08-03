using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Widget
{
    public class StartChatViewModel
    {
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }      

        [JsonProperty("operatorName")]
        public string OperatorName { get; set; }

        [JsonProperty("messages")]
        public List<MessageViewModel> Messages { get; set; }
    }
}