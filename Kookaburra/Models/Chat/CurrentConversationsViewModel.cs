using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Chat
{
    public class CurrentConversationsViewModel
    {
        [JsonProperty("conversations")]
        public List<OperatorConversationViewModel> Conversations { get; set; }
    }
}