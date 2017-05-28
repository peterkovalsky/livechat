using System.Collections.Generic;

namespace Kookaburra.Domain.Query.CurrentChats
{
    public class CurrentChatsQueryResult
    {
        public List<ChatInfoResult> CurrentChats { get; set; }

        public int UnreadMessages { get; set; }
    }

    public class ChatInfoResult
    {
        public string VisitorSessionId { get; set; }
    }
}