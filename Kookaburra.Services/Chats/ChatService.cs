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

        public async Task ConnectOperatorAsync(string accountKey, string operatorIdentity, string operatorConnectionId)
        {
            var operatorEntity = await _context.Operators
               .Include(i => i.Account)
               .Where(o => o.Identifier == operatorIdentity && o.Account.Identifier == accountKey)
               .SingleOrDefaultAsync();

            _chatSession.AddOrUpdateOperator(operatorEntity.Id, operatorEntity.Identifier, operatorEntity.FirstName, operatorEntity.Account.Identifier, operatorConnectionId);
        }

        public async Task ResumeOperatorAsync(string operatorIdentity)
        {
            var operatorSession = _chatSession.GetOperatorByIdentity(operatorIdentity);

            if (operatorSession != null)
            {            
                var liveConversations = operatorSession.Visitors.Select(v => v.ConversationId).ToList();

                var conversations = await _context.Conversations
                                            .Where(c => c.Operator.Identifier == operatorIdentity
                                                    && liveConversations.Contains(c.Id))
                                            .Select(c => new ConversationResult
                                            {
                                                VisitorInfo = new VisitorInfoResult
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
                                                Messages = c.Messages.Select(m => new MessageResult
                                                {
                                                    Author = m.SentBy == UserType.Visitor.ToString() ? c.Visitor.Name : c.Operator.FirstName,
                                                    Text = m.Text,
                                                    SentOn = m.DateSent,
                                                    SentBy = m.SentBy.ToLower()
                                                }).ToList()
                                            }).ToListAsync();

                return new ResumeOperatorQueryResult { Conversations = conversations };
            }

            return null;
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

        public async Task VisitorMessagedAsync(string visitorConnectionId, string message, DateTime dateSent)
        {
            var visitorSession = _chatSession.GetVisitorByVisitorConnId(visitorConnectionId);          

            _context.Messages.Add(new Message
            {
                ConversationId = visitorSession.ConversationId,
                SentBy = UserType.Visitor.ToString(),
                Text = message,
                DateSent = dateSent
            });

            await _context.SaveChangesAsync();
        }

        public async Task<long> VisitorStartChatAsync(VisitorStartChatRequest request)
        {
            // record new/returning visitor
            var returningVisitor = await CheckForVisitorAsync(request.VisitorName, request.VisitorEmail, request.VisitorKey);

            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Identifier == request.AccountKey);
            if (account == null)
            {
                throw new ArgumentException($"Account {request.AccountKey} doesn't exist.");
            }

            // new visitor
            if (returningVisitor == null)
            {
                returningVisitor = new Visitor
                {
                    Name = request.VisitorName,
                    Email = request.VisitorEmail,
                    Identifier = request.VisitorKey,
                    IpAddress = request.VisitorIP,
                    Account = account
                };

                _context.Visitors.Add(returningVisitor);
            }
            else // update visitor details
            {
                returningVisitor.Name = request.VisitorName;
                returningVisitor.Email = request.VisitorEmail;
                returningVisitor.Identifier = request.VisitorKey;
                returningVisitor.IpAddress = request.VisitorIP;
            }

            var operatorSession = _chatSession.GetOperatorById(request.OperatorId);

            var conversation = new Conversation
            {
                OperatorId = operatorSession.Id,
                Visitor = returningVisitor,
                TimeStarted = DateTime.UtcNow,
                Page = request.Page
            };
            _context.Conversations.Add(conversation);

            await _context.SaveChangesAsync();
                     

            // add visitor to session
            _chatSession.AddVisitor(conversation.Id, request.OperatorId, null, returningVisitor.Id, returningVisitor.Name, returningVisitor.Identifier);

            // add greeting if needed   
            await OperatorMessagedAsync(request.VisitorKey, DefaultSettings.CHAT_GREETING, DateTime.UtcNow);

            // return visitor id
            return returningVisitor.Id;
        }

        private async Task<Visitor> CheckForVisitorAsync(string name, string email, string visitorKey)
        {
            var existingVisitor = await _context.Visitors
                .Where(v => v.Identifier == visitorKey)
                .SingleOrDefaultAsync();

            return existingVisitor;
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

        public async Task DisconnectOperatorAsync(string connectionId)
        {
            var operatorSession = _chatSession.GetOperatorByOperatorConnId(connectionId);
            var conversationIds = operatorSession.Visitors.Select(v => v.ConversationId).ToList();

            if (_chatSession.DisconnectOperator(connectionId))
            {
                var conversations = await _context.Conversations.Where(c => conversationIds.Contains(c.Id)).ToListAsync();
            }
        }

        public async Task StopChatAsync(string visitorIndentity, long conversationId = default(long))
        {         
            if (conversationId == default(long))
            {
                var visitorSession = _chatSession.GetVisitorByVisitorSessionId(visitorIndentity);
                if (visitorSession == null) return;

                conversationId = visitorSession.ConversationId;
            }

            var conversation = await _context.Conversations.Where(c => c.Id == conversationId).SingleOrDefaultAsync();

            if (conversation == null)
            {
                throw new ArgumentException("There is no conversation for visitor " + visitorIndentity);
            }

            conversation.TimeFinished = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _chatSession.RemoveVisitor(visitorIndentity);
        }
    }
}