using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.CurrentChats
{
    public class CurrentChatsQueryHandler : IQueryHandler<CurrentChatsQuery, Task<CurrentChatsQueryResult>>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public CurrentChatsQueryHandler(ChatSession chatSession, KookaburraContext context)
        {
            _chatSession = chatSession;
            _context = context;
        }

        public async Task<CurrentChatsQueryResult> ExecuteAsync(CurrentChatsQuery query)
        {
            var operatorSession = _chatSession.GetOperatorByIdentity(query.OperatorIdentity);

            return new CurrentChatsQueryResult
            {
                CurrentChats = operatorSession.Visitors.Select(v => new ChatInfoResult { VisitorSessionId = v.SessionId }).ToList(),
                UnreadMessages = await _context.OfflineMessages
                                         .Where(om =>
                                                om.Account.Identifier == query.AccountKey
                                            && !om.IsRead)
                                         .CountAsync()
            };
        }
    }
}