using System.Threading.Tasks;

namespace Kookaburra.Services.OfflineMessages
{
    public interface IOfflineMessageService
    {
        Task<int> TotalNewOfflineMessagesAsync(string operatorIdentity);
    }
}