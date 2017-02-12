using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Chat
{
    public class OperatorCurrentChatsViewModel
    {
        [JsonProperty("currentChats")]
        public List<string> CurrentChats { get; set; }
    }
}