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
        public string Author { get; set; }

        public string Text { get; set; }

        public DateTime Time { get; set; }
    }
}