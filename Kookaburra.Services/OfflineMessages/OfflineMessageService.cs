using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using Kookaburra.Services.Accounts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services.OfflineMessages
{
    public class OfflineMessageService : IOfflineMessageService
    {        
        private readonly KookaburraContext _context;
        private readonly IAccountService _accountService;

        public OfflineMessageService(KookaburraContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public async Task<int> TotalNewOfflineMessagesAsync(string operatorKey)
        {
            var account = await _accountService.GetAccountAsync(operatorKey);

            return await _context.OfflineMessages.Where(om =>
                                                        om.Visitor.Account.Identifier == account.Identifier
                                                    && !om.IsRead)
                                                 .CountAsync();
        }

        public async Task<List<OfflineMessage>> GetOfflineMessagesAsync(TimeFilterType timeFilter, string operatorKey, Pagination pagination = null)
        {
            var account = await _accountService.GetAccountAsync(operatorKey);            

            var offlineMessages = _context.OfflineMessages.Include(i => i.Visitor).Where(om => om.Visitor.Account.Identifier == account.Identifier);

            if (timeFilter == TimeFilterType.Today)
            {
                var aDayAgo = DateTime.UtcNow.AddDays(-1);

                offlineMessages = offlineMessages.Where(om => aDayAgo <= om.DateSent);
            }
            else if (timeFilter == TimeFilterType.Week)
            {
                var aWeekAgo = DateTime.UtcNow.AddDays(-7);

                offlineMessages = offlineMessages.Where(om => aWeekAgo <= om.DateSent);
            }
            else if (timeFilter == TimeFilterType.Fortnight)
            {
                var aFortnightAgo = DateTime.UtcNow.AddDays(-14);

                offlineMessages = offlineMessages.Where(om => aFortnightAgo <= om.DateSent);
            }
            else if (timeFilter == TimeFilterType.Month)
            {
                var aMonthAgo = DateTime.UtcNow.AddMonths(-1);

                offlineMessages = offlineMessages.Where(om => aMonthAgo <= om.DateSent);
            }
            else if (timeFilter == TimeFilterType.Year)
            {
                var aYearAgo = DateTime.UtcNow.AddYears(-1);

                offlineMessages = offlineMessages.Where(om => aYearAgo <= om.DateSent);
            }
            else if (timeFilter == TimeFilterType.All)
            {
            }
            
            if (pagination != null)
            {
                pagination.Total = await offlineMessages.CountAsync();

                offlineMessages = offlineMessages.OrderByDescending(om => om.DateSent).Skip(pagination.Skip).Take(pagination.Size);
            }

            return await offlineMessages.ToListAsync();
        }

        public async Task<List<OfflineMessage>> SearchOfflineMessagesAsync(string query, string operatorKey, Pagination pagination = null)
        {
            var account = await _accountService.GetAccountAsync(operatorKey);

            var offlineMessages = _context.OfflineMessages
                                          .Include(i => i.Visitor)
                                          .Where(om =>
                                                om.Visitor.Account.Identifier == account.Identifier &&
                                                (om.Message.Contains(query) ||
                                                om.Visitor.Name.Contains(query) ||
                                                om.Visitor.Email.Contains(query)));
           
            if (pagination != null)
            {
                pagination.Total = await offlineMessages.CountAsync();

                offlineMessages = offlineMessages.OrderByDescending(om => om.DateSent).Skip(pagination.Skip).Take(pagination.Size);
            }

            return await offlineMessages.ToListAsync();
        }

        public async Task MarkMessageAsReadAsync(long messageId, string operatorKey)
        {
            var account = await _accountService.GetAccountAsync(operatorKey);

            var message = await _context.OfflineMessages.Where(om =>
                                                             om.Id == messageId
                                                          && om.Visitor.Account.Identifier == account.Identifier)
                                                      .SingleOrDefaultAsync();

            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteMessageAsync(long messageId, string operatorKey)
        {
            var account = await _accountService.GetAccountAsync(operatorKey);

            var message = await _context.OfflineMessages.Where(om =>
                                                           om.Id == messageId
                                                        && om.Visitor.Account.Identifier == account.Identifier)
                                                       .SingleOrDefaultAsync();

            if (message != null)
            {
                _context.OfflineMessages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}