using Kookaburra.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public interface IChatService
    {
        Task<List<Conversation>> GetVisitorQueueAsync(string operatorKey);

        Task<List<Conversation>> GetLiveChatsAsync(string operatorKey);
    }
}