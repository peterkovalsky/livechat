using System;
using System.Collections.Generic;

namespace Kookaburra.Domain.Query.Result
{
    public class ContinueConversationQueryResult
    {
        public bool IsNewConversation { get; set; }

        public OperatorInfo OperatorInfo { get; set; }

        public VisitorInfo VisitorInfo { get; set; }

        public List<ConversationItem> Conversation { get; set; }
    }

    public class ConversationItem
    {
        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime Time { get; set; }

        public string SentBy { get; set; }
    }

    public class VisitorInfo
    {
        public string Name { get; set; }

        public string Page { get; set; }

        public string Location { get; set; }
    }

    public class OperatorInfo
    {
        public string Name { get; set; }

        public string ConnectionId { get; set; }

        public string SessionId { get; set; }
    }
}