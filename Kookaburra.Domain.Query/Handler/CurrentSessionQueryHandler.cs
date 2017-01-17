using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using System.Linq;

namespace Kookaburra.Domain.Query.Handler
{
    public class CurrentSessionQueryHandler : IQueryHandler<CurrentSessionQuery, CurrentSessionQueryResult>
    {
        private readonly ChatSession _chatSession;

        public CurrentSessionQueryHandler(ChatSession chatSession)
        {
            _chatSession = chatSession;
        }

        public CurrentSessionQueryResult Execute(CurrentSessionQuery query)
        {
            if (!string.IsNullOrWhiteSpace(query.VisitorConnectionId))
            {
                var operatorSession = _chatSession.GetOperatorByVisitorConnId(query.VisitorConnectionId);
                var visitorSession = _chatSession.GetVisitorByVisitorConnId(query.VisitorConnectionId);

                return new CurrentSessionQueryResult
                {
                    VisitorConnectionId = visitorSession.ConnectionId,
                    VisitorSessionId = visitorSession.SessionId,
                    OperatorConnectionId = operatorSession.ConnectionId,
                    OperatorSessionId = operatorSession.SessionId
                };
            }

            if (!string.IsNullOrWhiteSpace(query.VisitorSessionId))
            {
                var operatorSession = _chatSession.GetOperatorByVisitorSessionId(query.VisitorSessionId);
                var visitorSession = _chatSession.GetVisitorByVisitorSessionId(query.VisitorSessionId);

                return new CurrentSessionQueryResult
                {
                    VisitorConnectionId = visitorSession.ConnectionId,
                    VisitorSessionId = visitorSession.SessionId,
                    OperatorConnectionId = operatorSession.ConnectionId,
                    OperatorSessionId = operatorSession.SessionId
                };
            }

            return null;
        }
    }
}