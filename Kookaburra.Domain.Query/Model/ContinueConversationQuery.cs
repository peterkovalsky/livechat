using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class ContinueConversationQuery : IQuery<ContinueConversationQueryResult>
    {
        public ContinueConversationQuery(string sessionId, string visitorConnectionId)
        {
            SessionId = sessionId;
            VisitorConnectionId = visitorConnectionId;
        }

        public string SessionId { get; private set; }

        public string VisitorConnectionId { get; private set; }
    }
}