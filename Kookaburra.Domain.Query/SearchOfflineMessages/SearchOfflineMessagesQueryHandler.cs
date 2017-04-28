using Kookaburra.Domain.Query.OfflineMessages;
using Kookaburra.Repository;
using System.Linq;

namespace Kookaburra.Domain.Query.SearchOfflineMessages
{
    public class SearchOfflineMessagesQueryHandler : IQueryHandler<SearchOfflineMessagesQuery, OfflineMessagesQueryResult>
    {
        private readonly KookaburraContext _context;

        public SearchOfflineMessagesQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public OfflineMessagesQueryResult Execute(SearchOfflineMessagesQuery query)
        {
            var offlineMessages = _context.OfflineMessages.Where(om =>
                                        om.Account.Operators.Any(o => o.Identity == query.OperatorIdentity) &&
                                        (om.Message.Contains(query.Query) ||
                                        om.Visitor.Name.Contains(query.Query) ||
                                        om.Visitor.Email.Contains(query.Query)));

            var total = offlineMessages.Count();

            if (query.Pagination != null)
            {
                offlineMessages = offlineMessages.OrderByDescending(om => om.DateSent).Skip(query.Pagination.Skip).Take(query.Pagination.Size);
            }

            return new OfflineMessagesQueryResult
            {
                TotalMessages = total,

                OfflineMessages = offlineMessages.Select(om => new OfflineMessageResult
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
                }).ToList()
            };
        }
    }
}