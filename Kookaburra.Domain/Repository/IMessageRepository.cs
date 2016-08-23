using Kookaburra.Domain.Model;

namespace Kookaburra.Domain.Repository
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
    }
} 