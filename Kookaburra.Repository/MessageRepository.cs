using Kookaburra.Domain.Model;
using Kookaburra.Domain.Repository;
using System.Collections.Generic;
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

        //public IEnumerable<Message> GetHistoricalChats(string operatorIdentity, int size, int page)
        //{
        //    _context.Messages
        //        .Where(m => m.Operator.Identity == operatorIdentity)
        //        .GroupBy(m => m.VisitorId)
        //        .Select(g => new { VisitorId = g.Key, Messages = g,  }).Take(size).Skip(size * page)
        //}
    }
}