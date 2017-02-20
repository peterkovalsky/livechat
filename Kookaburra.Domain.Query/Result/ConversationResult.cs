using System.Collections.Generic;

namespace Kookaburra.Domain.Query.Result
{
    public class ConversationResult
    {
        public VisitorInfoResult VisitorInfo { get; set; }

        public List<MessageResult> Messages { get; set; }
    }
}