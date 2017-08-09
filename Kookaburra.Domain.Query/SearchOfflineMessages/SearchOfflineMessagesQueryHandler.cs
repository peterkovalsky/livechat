using Kookaburra.Domain.Query.OfflineMessages;
using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.SearchOfflineMessages
{
    public class SearchOfflineMessagesQueryHandler : IQueryHandler<SearchOfflineMessagesQuery, Task<OfflineMessagesQueryResult>>
    {
        private readonly KookaburraContext _context;

        public SearchOfflineMessagesQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public async Task<OfflineMessagesQueryResult> ExecuteAsync(SearchOfflineMessagesQuery query)
        {
            var offlineMessages = _context.OfflineMessages.Where(om =>
                                        om.Account.Identifier == query.AccountKey &&
                                        (om.Message.Contains(query.Query) ||
                                        om.Visitor.Name.Contains(query.Query) ||
                                        om.Visitor.Email.Contains(query.Query)));

            var total = await offlineMessages.CountAsync();

            if (query.Pagination != null)
            {
                offlineMessages = offlineMessages.OrderByDescending(om => om.DateSent).Skip(query.Pagination.Skip).Take(query.Pagination.Size);
            }

            return new OfflineMessagesQueryResult
            {
                TotalMessages = total,

                OfflineMessages = await offlineMessages.Select(om => new OfflineMessageResult
                {
                    Id = om.Id,
                    VisitorName = om.Visitor.Name,
                    Email = om.Visitor.Email,
                    Message = om.Message,
                    IsRead = om.IsRead,
                    TimeSent = om.DateSent,
                    Country = om.Visitor.Country,
                    CountryCode = om.Visitor.CountryCode,
                    City = om.Visitor.City
                }).ToListAsync()
            };
        }
    }
}