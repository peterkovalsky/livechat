using System.Collections.Generic;

namespace Kookaburra.Domain.Query.Result
{
    public class ContinueConversationQueryResult
    {
        public bool IsNewConversation { get; set; }

        public OperatorInfo OperatorInfo { get; set; }

        public VisitorInfoResult VisitorInfo { get; set; }

        public List<MessageResult> Conversation { get; set; }
    }

    public class OperatorInfo
    {
        public string Name { get; set; }

        public List<string> ConnectionIds { get; set; }        
    }
}