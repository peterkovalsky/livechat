using System;
using System.Collections.Generic;

namespace Kookaburra.Services.Chats
{
    public class ChatHistoryResponse
    {
        public List<ConversationItemResponse> Conversations { get; set; }

        public int TotalConversations { get; set; }
    }

    public class ConversationItemResponse
    {
        public long Id { get; set; }

        public string VisitorName { get; set; }

        public string OperatorName { get; set; }

        public string Text { get; set; }

        public DateTime StartTime { get; set; }

        public int TotalMessages { get; set; }
    }
}