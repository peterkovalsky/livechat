﻿using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.CurrentSession
{
    public class CurrentSessionQueryHandler : IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>>
    {
        private readonly ChatSession _chatSession;

        public CurrentSessionQueryHandler(ChatSession chatSession)
        {
            _chatSession = chatSession;
        }

        public async Task<CurrentSessionQueryResult> ExecuteAsync(CurrentSessionQuery query)
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
                    VisitorConnectionIds = visitorSession.ConnectionIds,
                    VisitorSessionId = visitorSession.SessionId,
                    OperatorName = operatorSession.Name,
                    OperatorConnectionIds = operatorSession.ConnectionIds               
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
                    VisitorConnectionIds = visitorSession.ConnectionIds,
                    VisitorSessionId = visitorSession.SessionId,
                    OperatorName = operatorSession.Name,
                    OperatorConnectionIds = operatorSession.ConnectionIds
                };
            }

            return null;
        }
    }
}