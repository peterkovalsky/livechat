using System.Collections.Generic;

namespace Kookaburra.Domain.Query.TimmedOutConversations
{
    public class TimmedOutConversationsQueryResult
    {
        public List<ConversationResult> Conversations { get; set; }
    }

    public class ConversationResult
    {
        public string VisitorSessionId { get; set; }

        public long ConversationId { get; set; }
    }
}