using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Widget
{
    public class ConversationViewModel
    {
        [JsonProperty("conversation")]
        public List<MessageViewModel> Conversation { get; set; }
    }
}