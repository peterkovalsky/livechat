using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Repository;
using System.Linq;

namespace Kookaburra.Domain.Query.Handler
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
                                        om.Message.Contains(query.Query) ||
                                        om.Visitor.Name.Contains(query.Query) ||
                                        om.Visitor.Email.Contains(query.Query));

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
                    VisitorName = om.Visitor.Name,
                    Email = om.Visitor.Email,
                    Message = om.Message,
                    TimeSent = om.DateSent,
                    Country = om.Visitor.Country,
                    City = om.Visitor.City
                }).ToList()
            };
        }
    }
}