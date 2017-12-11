using Kookaburra.Domain;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using Kookaburra.Services.Accounts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public class OperatorChatService : IOperatorChatService
    {
        private readonly KookaburraContext _context;      
        private readonly ChatSession _chatSession;
        private readonly IAccountService _accountService;

        public OperatorChatService(KookaburraContext context, ChatSession chatSession, IAccountService accountService)
        {
            _context = context;           
            _chatSession = chatSession;
            _accountService = accountService;
        }


        public async Task ConnectOperatorAsync(string operatorIdentity, string operatorConnectionId)
        {
            var account = await _accountService.GetAccountForOperatorAsync(operatorIdentity);

            if (account.AccountStatus == AccountStatusType.Paid || account.AccountStatus == AccountStatusType.Trial)
            {
                var operatorEntity = await _context.Operators
                   .Include(i => i.Account)
                   .Where(o => o.Identifier == operatorIdentity)
                   .SingleOrDefaultAsync();

                _chatSession.AddOrUpdateOperator(operatorEntity.Id, operatorEntity.Identifier, operatorEntity.FirstName, operatorEntity.Account.Key, operatorConnectionId);
            }
        }

        public async Task<List<ConversationResponse>> ResumeOperatorAsync(string operatorIdentity)
        {
            var operatorSession = _chatSession.GetOperatorByIdentity(operatorIdentity);

            if (operatorSession != null)
            {
                var liveConversations = operatorSession.Visitors.Select(v => v.ConversationId).ToList();

                var conversations = await _context.Conversations
                                            .Where(c => c.Operator.Identifier == operatorIdentity
                                                    && liveConversations.Contains(c.Id))
                                            .Select(c => new ConversationResponse
                                            {
                                                VisitorInfo = new VisitorInfoResponse
                                                {
                                                    SessionId = c.Visitor.Identifier,
                                                    Name = c.Visitor.Name,
                                                    Email = c.Visitor.Email,
                                                    CurrentUrl = c.Page,
                                                    StartTime = c.TimeStarted,
                                                    Country = c.Visitor.Country,
                                                    City = c.Visitor.City,
                                                    Region = c.Visitor.Region,
                                                    Latitude = c.Visitor.Latitude,
                                                    Longitude = c.Visitor.Longitude
                                                },
                                                Messages = c.Messages.Select(m => new MessageResponse
                                                {
                                                    Author = m.SentBy == UserType.Visitor.ToString() ? c.Visitor.Name : c.Operator.FirstName,
                                                    Text = m.Text,
                                                    SentOn = m.DateSent,
                                                    SentBy = m.SentBy.ToLower()
                                                }).ToList()
                                            }).ToListAsync();

                return conversations;
            }

            return null;
        }

        public async Task<CurrentChatsResponse> GetCurrentChatsAsync(string operatorIdentity)
        {
            var currentChats = new List<ChatInfoResponse>();

            var operatorSession = _chatSession.GetOperatorByIdentity(operatorIdentity);
            var account = await _accountService.GetAccountForOperatorAsync(operatorIdentity);

            if (operatorSession != null)
            {
                currentChats = operatorSession.Visitors.Select(v => new ChatInfoResponse { VisitorSessionId = v.SessionId }).ToList();
            }
            return new CurrentChatsResponse
            {
                CurrentChats = currentChats,
                UnreadMessages = await _context.OfflineMessages
                                         .Where(om =>
                                                om.Visitor.Account.Key == account.Key
                                            && !om.IsRead)
                                         .CountAsync(),
                IsPaid = account.AccountStatus == AccountStatusType.Paid
            };
        }

        public async Task OperatorMessagedAsync(string visitorIdentity, string message, DateTime dateSent, UserType sentBy = UserType.Operator)
        {
            var visitorSession = _chatSession.GetVisitorByVisitorSessionId(visitorIdentity);
            if (visitorSession == null)
            {
                throw new ArgumentException(string.Format("Visitor with session {0} doesn't exist.", visitorIdentity));
            }

            _context.Messages.Add(new Message
            {
                ConversationId = visitorSession.ConversationId,
                SentBy = sentBy.ToString(),
                Text = message,
                DateSent = dateSent
            });

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a list of new chats which operator hasn't replied to.
        /// </summary>     
        public async Task<List<Conversation>> GetVisitorQueueAsync(string operatorIdentity)
        {
            // all new chats associated with the operator
            var chatsInQueue = await _context.Conversations.Include(i => i.Visitor)
                                              .Where(c => c.Operator.Identifier == operatorIdentity
                                                       && c.Messages.All(m => m.SentBy != UserType.Operator.ToString())
                                                       && c.TimeFinished == null)
                                              .ToListAsync();

            // only real-time chats
            var operatorSession = _chatSession.GetOperatorByIdentity(operatorIdentity);
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
        public async Task<List<Conversation>> GetLiveChatsAsync(string operatorIdentity)
        {
            // only real-time chats
            var operatorSession = _chatSession.GetOperatorByIdentity(operatorIdentity);
            if (operatorSession == null)
            {
                return new List<Conversation>();
            }

            var liveConversations = operatorSession.Visitors.Select(v => v.ConversationId).ToList();

            return await _context.Conversations
                                            .Include(i => i.Visitor)
                                            .Include(i => i.Messages)
                                            .Where(c => c.Operator.Identifier == operatorIdentity && liveConversations.Contains(c.Id))
                                            .ToListAsync();
        }

        public async Task DisconnectOperatorAsync(string operatorConnectionId)
        {
            var operatorSession = _chatSession.GetOperatorByOperatorConnId(operatorConnectionId);
            var conversationIds = operatorSession.Visitors.Select(v => v.ConversationId).ToList();

            if (_chatSession.DisconnectOperator(operatorConnectionId))
            {
                var conversations = await _context.Conversations.Where(c => conversationIds.Contains(c.Id)).ToListAsync();
            }
        }
    }
}