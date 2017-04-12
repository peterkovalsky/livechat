using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.History
{
    public class ChatHistoryViewModel
    {
        [JsonProperty("conversations")]
        public List<ConversationItemViewModel> Conversations { get; set; }

        [JsonProperty("totalConversations")]
        public int TotalConversations { get; set; }
    }
}