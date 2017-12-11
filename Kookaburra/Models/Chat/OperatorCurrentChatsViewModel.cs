using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kookaburra.Models.Chat
{
    public class OperatorCurrentChatsViewModel
    {
        [JsonProperty("currentChats")]
        public List<ChatInfo> CurrentChats { get; set; }

        [JsonProperty("unreadMessages")]
        public int UnreadMessages { get; set; }

        [JsonProperty("isPaid")]
        public bool IsPaid { get; set; }
    }

    public class ChatInfo
    {
        [JsonProperty("visitorSessionId")]
        public string VisitorSessionId { get; set; }
    }
}