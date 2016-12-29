using System;
using System.Collections.Generic;

namespace Kookaburra.Domain.Query.Result
{
    public class ContinueConversationQueryResult
    {
        public List<ConversationItem> Conversation { get; set; }
    }

    public class ConversationItem
    {
        public string Name { get; set; }

        public string Message { get; set; }

        public DateTime TimeSent { get; set; }
    }
}