using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using System.Linq;

namespace Kookaburra.Domain.Query.Handler
{
    public class CurrentChatsQueryHandler : IQueryHandler<CurrentChatsQuery, CurrentChatsQueryResult>
    {
        private readonly ChatSession _chatSession;

        public CurrentChatsQueryHandler(ChatSession chatSession)
        {
            _chatSession = chatSession;
        }

        public CurrentChatsQueryResult Execute(CurrentChatsQuery query)
        {
            var operatorSession = _chatSession.GetOperatorByIdentity(query.OperatorIdentity);

            return new CurrentChatsQueryResult
            {
                CurrentChats = operatorSession.Visitors.Select(v => new ChatInfoResult { VisitorSessionId = v.SessionId }).ToList()
            };
        }
    }
}