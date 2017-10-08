using Kookaburra.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public interface IChatService
    {
        Task<List<Conversation>> GetVisitorQueueAsync(string operatorIdentity);

        Task<List<Conversation>> GetLiveChatsAsync(string operatorIdentity);
    }
}