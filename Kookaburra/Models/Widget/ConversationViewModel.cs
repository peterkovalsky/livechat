using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Widget
{
    public class ConversationViewModel
    {
        [JsonProperty("visitorName")]
        public string VisitorName { get; set; }

        [JsonProperty("operatorName")]
        public string OperatorName { get; set; }

        [JsonProperty("conversation")]
        public List<MessageViewModel> Conversation { get; set; }
    }
}