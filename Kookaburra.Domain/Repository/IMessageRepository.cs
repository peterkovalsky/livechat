using Kookaburra.Domain.Model;
using System.Collections.Generic;

namespace Kookaburra.Domain.Repository
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);

        void AddOfflineMessage(OfflineMessage offlineMsg);

        IEnumerable<Conversation> GetHistoricalChats(string operatorIdentity, int size, int page);
    }
}