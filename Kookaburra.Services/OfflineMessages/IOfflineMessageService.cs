using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Services.OfflineMessages
{
    public interface IOfflineMessageService
    {
        Task<long> LeaveMessage(OfflineMessage message, Visitor visitor, string accountKey);

        Task<int> TotalNewOfflineMessagesAsync(string operatorKey);

        Task<List<OfflineMessage>> GetOfflineMessagesAsync(TimeFilterType timeFilter, string operatorKey, Pagination pagination = null);

        Task<List<OfflineMessage>> SearchOfflineMessagesAsync(string query, string operatorKey, Pagination pagination = null);

        Task MarkMessageAsReadAsync(long messageId, string operatorKey);

        Task DeleteMessageAsync(long messageId, string operatorKey);
    }
}