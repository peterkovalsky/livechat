using Kookaburra.Domain.Model;
using Kookaburra.Domain.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

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

        public void AddOfflineMessage(OfflineMessage offlineMsg)
        {
            _context.OfflineMessages.Add(offlineMsg);
            _context.SaveChanges();
        }

        public IEnumerable<Visitor> GetHistoricalChats(string operatorIdentity, int size, int page)
        {
            return _context.Visitors
                .Include(i => i.Messages)
                .OrderByDescending(v => v.ConversationStarted)
                .Skip((page - 1) * size).Take(size)
                .ToList();
        }
    }
}