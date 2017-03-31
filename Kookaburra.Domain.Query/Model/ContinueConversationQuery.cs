using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class ContinueConversationQuery : IQuery<ContinueConversationQueryResult>
    {
        public ContinueConversationQuery(string visitorSessionId, string visitorConnectionId, string operatorIdentity)
        {
            VisitorSessionId = visitorSessionId;
            VisitorConnectionId = visitorConnectionId;
            OperatorIdentity = operatorIdentity;
        }

        public string VisitorSessionId { get; }

        public string VisitorConnectionId { get; }

        public string OperatorIdentity { get; }
    }
}