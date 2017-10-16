using Kookaburra.Domain;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using Kookaburra.Services.Accounts;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public class ChatService : IChatService
    {
        private readonly KookaburraContext _context;
        private readonly IAccountService _accountService;
        private readonly ChatSession _chatSession;

        public ChatService(KookaburraContext context, IAccountService accountService, ChatSession chatSession)
        {
            _context = context;
            _accountService = accountService;
            _chatSession = chatSession;
        }

        /// <summary>
        /// Gets a list of new chats which operator hasn't replied to.
        /// </summary>     
        public async Task<List<Conversation>> GetVisitorQueueAsync(string operatorKey)
        {
            // all new chats associated with the operator
            var chatsInQueue = await _context.Conversations.Include(i => i.Visitor)
                                              .Where(c => c.Operator.Identifier == operatorKey
                                                       && c.Messages.All(m => m.SentBy != UserType.Operator.ToString())
                                                       && c.TimeFinished == null)
                                              .ToListAsync();

            // only real-time chats
            var operatorSession = _chatSession.GetOperatorByIdentity(operatorKey);
            if (operatorSession == null)
            {
                return new List<Conversation>();
            }

            var operatorChats = operatorSession.Visitors.Select(v => v.ConversationId).ToList();

            return chatsInQueue.Where(c => operatorChats.Contains(c.Id)).ToList();
        }

        /// <summary>
        /// Gets a list of all current chats including the ones operator hasn't responded yet.
        /// </summary>     
        public async Task<List<Conversation>> GetLiveChatsAsync(string operatorKey)
        {
            // only real-time chats
            var operatorSession = _chatSession.GetOperatorByIdentity(operatorKey);
            if (operatorSession == null)
            {
                return new List<Conversation>();
            }

            var liveConversations = operatorSession.Visitors.Select(v => v.ConversationId).ToList();

            return await _context.Conversations
                                            .Include(i => i.Visitor)
                                            .Include(i => i.Messages)
                                            .Where(c => c.Operator.Identifier == operatorKey && liveConversations.Contains(c.Id))
                                            .ToListAsync();    
        }
    }
}