using Kookaburra.Domain.Model;
using Kookaburra.Domain.Repository;

namespace Kookaburra.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private KookaburraContext _context;

        public MessageRepository(KookaburraContext context)
        {
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();
        }
    }
} 