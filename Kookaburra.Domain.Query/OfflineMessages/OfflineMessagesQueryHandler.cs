using Kookaburra.Domain.Common;
using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.OfflineMessages
{
    public class OfflineMessagesQueryHandler : IQueryHandler<OfflineMessagesQuery, Task<OfflineMessagesQueryResult>>
    {
        private readonly KookaburraContext _context;

        public OfflineMessagesQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public async Task<OfflineMessagesQueryResult> ExecuteAsync(OfflineMessagesQuery query)
        {
            var account = await _context.Accounts.Where(a => a.Operators.Any(o => o.Identifier == query.OperatorKey)).SingleOrDefaultAsync();

            var offlineMessages = _context.OfflineMessages.Where(om => om.Visitor.Account.Identifier == account.Identifier);       

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