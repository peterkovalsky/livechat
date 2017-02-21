using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Chat
{
    public class OperatorConversationViewModel
    {
        [JsonProperty("visitor")]
        public VisitorInfoViewModel VisitorInfo { get; set; }

        [JsonProperty("messages")]
        public List<MessageViewModel> Messages { get; set; }
    }
}