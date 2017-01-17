using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class ContinueConversationQuery : IQuery<ContinueConversationQueryResult>
    {
        public ContinueConversationQuery(string visitorSessionId, string visitorConnectionId)
        {
            VisitorSessionId = visitorSessionId;
            VisitorConnectionId = visitorConnectionId;
        }

        public string VisitorSessionId { get; private set; }

        public string VisitorConnectionId { get; private set; }
    }
}