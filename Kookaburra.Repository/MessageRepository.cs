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
            
        }

        public void AddOfflineMessage(OfflineMessage offlineMsg)
        {
            _context.OfflineMessages.Add(offlineMsg);
            _context.SaveChanges();
        }

        public IEnumerable<Conversation> GetHistoricalChats(string operatorIdentity, int size, int page)
        {
            return _context.Conversations
                .Include(i => i.Messages)
                .OrderByDescending(v => v.TimeStarted)
                .Skip((page - 1) * size).Take(size)
                .ToList();
        }
    }
}