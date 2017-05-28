using Kookaburra.Domain.ResumeVisitorChat;
using System.Collections.Generic;

namespace Kookaburra.Domain.Query.ResumeOperator
{
    public class ConversationResult
    {
        public VisitorInfoResult VisitorInfo { get; set; }

        public List<MessageResult> Messages { get; set; }
    }
}