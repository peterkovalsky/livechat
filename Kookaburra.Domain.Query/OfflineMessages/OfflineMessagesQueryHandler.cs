using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Repository;
using System;
using System.Linq;

namespace Kookaburra.Domain.Query.OfflineMessages
{
    public class OfflineMessagesQueryHandler : IQueryHandler<OfflineMessagesQuery, OfflineMessagesQueryResult>
    {
        private readonly KookaburraContext _context;

        public OfflineMessagesQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public OfflineMessagesQueryResult Execute(OfflineMessagesQuery query)
        {
            var offlineMessages = _context.OfflineMessages.Where(om => om.Account.Operators.Any(o => o.Identity == query.OperatorIdentity));       

            if (query.TimeFilter == TimeFilterType.Today)
            {
                var aDayAgo = DateTime.UtcNow.AddDays(-1);

                offlineMessages = offlineMessages.Where(om => aDayAgo <= om.DateSent);
            }
            else if (query.TimeFilter == TimeFilterType.Week)
            {
                var aWeekAgo = DateTime.UtcNow.AddDays(-7);

                offlineMessages = offlineMessages.Where(om => aWeekAgo <= om.DateSent);
            }
            else if (query.TimeFilter == TimeFilterType.Fortnight)
            {
                var aFortnightAgo = DateTime.UtcNow.AddDays(-14);

                offlineMessages = offlineMessages.Where(om => aFortnightAgo <= om.DateSent);
            }
            else if (query.TimeFilter == TimeFilterType.Month)
            {
                var aMonthAgo = DateTime.UtcNow.AddMonths(-1);

                offlineMessages = offlineMessages.Where(om => aMonthAgo <= om.DateSent);
            }
            else if (query.TimeFilter == TimeFilterType.Year)
            {
                var aYearAgo = DateTime.UtcNow.AddYears(-1);

                offlineMessages = offlineMessages.Where(om => aYearAgo <= om.DateSent);
            }
            else if (query.TimeFilter == TimeFilterType.All)
            {                
            }

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