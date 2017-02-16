using System.Collections.Generic;

namespace Kookaburra.Domain.Query.Result
{
    public class CurrentChatsQueryResult
    {
        public List<ChatInfoResult> CurrentChats { get; set; }
    }

    public class ChatInfoResult
    {
        public string VisitorSessionId { get; set; }
    }
}