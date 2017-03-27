using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Repository;
using System;
using System.Linq;

namespace Kookaburra.Domain.Query.Handler
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
            var offlineMessages = _context.OfflineMessages.AsQueryable();       

            if (query.TimeFilter == TimeFilterType.Day)
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