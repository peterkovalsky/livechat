using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;

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

                if (operatorSession == null || visitorSession == null)
                {
                    return null;
                }

                return new CurrentSessionQueryResult
                {
                    VisitorName = visitorSession.Name,
                    VisitorConnectionId = visitorSession.ConnectionId,
                    VisitorSessionId = visitorSession.SessionId,
                    OperatorName = operatorSession.Name,
                    OperatorConnectionId = operatorSession.ConnectionId,
                    OperatorSessionId = operatorSession.SessionId
                };
            }

            if (!string.IsNullOrWhiteSpace(query.VisitorSessionId))
            {

                var operatorSession = _chatSession.GetOperatorByVisitorSessionId(query.VisitorSessionId);
                var visitorSession = _chatSession.GetVisitorByVisitorSessionId(query.VisitorSessionId);

                if (operatorSession == null || visitorSession == null)
                {
                    return null;
                }

                return new CurrentSessionQueryResult
                {
                    VisitorName = visitorSession.Name,
                    VisitorConnectionId = visitorSession.ConnectionId,
                    VisitorSessionId = visitorSession.SessionId,
                    OperatorName = operatorSession.Name,
                    OperatorConnectionId = operatorSession.ConnectionId,
                    OperatorSessionId = operatorSession.SessionId
                };
            }

            return null;
        }
    }
}