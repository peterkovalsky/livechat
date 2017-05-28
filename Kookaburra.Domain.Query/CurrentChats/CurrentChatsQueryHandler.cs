using Kookaburra.Repository;
using System.Linq;

namespace Kookaburra.Domain.Query.CurrentChats
{
    public class CurrentChatsQueryHandler : IQueryHandler<CurrentChatsQuery, CurrentChatsQueryResult>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public CurrentChatsQueryHandler(ChatSession chatSession, KookaburraContext context)
        {
            _chatSession = chatSession;
            _context = context;
        }

        public CurrentChatsQueryResult Execute(CurrentChatsQuery query)
        {
            var operatorSession = _chatSession.GetOperatorByIdentity(query.OperatorIdentity);

            return new CurrentChatsQueryResult
            {
                CurrentChats = operatorSession.Visitors.Select(v => new ChatInfoResult { VisitorSessionId = v.SessionId }).ToList(),
                UnreadMessages = _context.OfflineMessages
                                         .Where(om =>
                                            om.Account.Operators.Any(o => o.Identity == query.OperatorIdentity)
                                            && !om.IsRead)
                                         .Count()
            };
        }
    }
}