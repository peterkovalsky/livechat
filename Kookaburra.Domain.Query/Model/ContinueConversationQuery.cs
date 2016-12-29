using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class ContinueConversationQuery : IQuery<ContinueConversationQueryResult>
    {
        public ContinueConversationQuery(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; private set; }
    }
}