using Kookaburra.Domain;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public class VisitorChatService : IVisitorChatService
    {
        private readonly KookaburraContext _context;

        private readonly ChatSession _chatSession;

        private readonly IOperatorChatService _operatorChatService;


        public VisitorChatService(KookaburraContext context, ChatSession chatSession, IOperatorChatService operatorChatService)
        {
            _context = context;
            _chatSession = chatSession;
            _operatorChatService = operatorChatService;
        }

        public async Task<ResumeVisitorChatResponse> ResumeVisitorChatAsync(string visitorIdentity, string visitorConnectionId)
        {
            if (string.IsNullOrWhiteSpace(visitorIdentity) || string.IsNullOrWhiteSpace(visitorConnectionId))
            {
                return null;
            }

            // check if conversation still alive 
            var visitorSession = _chatSession.GetVisitorByVisitorSessionId(visitorIdentity);
            if (visitorSession != null)
            {
                var conversation = await _context.Conversations
                    .Include(i => i.Messages)
                    .Include(i => i.Visitor)
                    .Include(i => i.Operator)
                    .Where(c => c.Id == visitorSession.ConversationId && c.TimeFinished == null)
                    .SingleOrDefaultAsync();

                // check if operator still there            
                if (conversation != null)
                {
                    _chatSession.UpdateVisitor(visitorIdentity, visitorConnectionId);

                    var conversationItems = conversation.Messages.Select(m => new MessageResponse
                    {
                        Author = m.SentBy == UserType.Visitor.ToString() ? conversation.Visitor.Name : conversation.Operator.FirstName,
                        Text = m.Text,
                        SentOn = m.DateSent,
                        SentBy = m.SentBy.ToLower()
                    }).ToList();

                    var operatorSession = _chatSession.GetOperatorByVisitorSessionId(visitorIdentity);

                    return new ResumeVisitorChatResponse
                    {
                        OperatorInfo = new OperatorInfoResponse
                        {
                            Name = conversation.Operator.FirstName,
                            ConnectionIds = operatorSession.ConnectionIds
                        },
                        VisitorInfo = new VisitorInfoResponse
                        {
                            Name = conversation.Visitor.Name,
                            Country = conversation.Visitor.Country,
                            City = conversation.Visitor.City,
                            CurrentUrl = conversation.Page
                        },
                        Conversation = conversationItems
                    };
                }
            }

            return null;
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
            var returningVisitor = await _context.Visitors.SingleOrDefaultAsync(v => v.Identifier == request.VisitorKey);

            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Key == request.AccountKey);
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
            await _operatorChatService.OperatorMessagedAsync(request.VisitorKey, DefaultSettings.CHAT_GREETING, DateTime.UtcNow);

            // return visitor id
            return returningVisitor.Id;
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

        public CurrentSessionResponse GetCurrentSessionByIdentity(string visitorIdentity)
        {
            if (!string.IsNullOrWhiteSpace(visitorIdentity))
            {
                var operatorSession = _chatSession.GetOperatorByVisitorSessionId(visitorIdentity);
                var visitorSession = _chatSession.GetVisitorByVisitorSessionId(visitorIdentity);

                if (operatorSession == null || visitorSession == null)
                {
                    return null;
                }

                return new CurrentSessionResponse
                {
                    VisitorName = visitorSession.Name,
                    VisitorConnectionIds = visitorSession.ConnectionIds,
                    VisitorIdentity = visitorSession.SessionId,
                    OperatorName = operatorSession.Name,
                    OperatorConnectionIds = operatorSession.ConnectionIds
                };
            }

            return null;
        }

        public CurrentSessionResponse GetCurrentSessionByConnection(string visitorConnectionId)
        {
            if (!string.IsNullOrWhiteSpace(visitorConnectionId))
            {
                var operatorSession = _chatSession.GetOperatorByVisitorConnId(visitorConnectionId);
                var visitorSession = _chatSession.GetVisitorByVisitorConnId(visitorConnectionId);

                if (operatorSession == null || visitorSession == null)
                {
                    return null;
                }

                return new CurrentSessionResponse
                {
                    VisitorName = visitorSession.Name,
                    VisitorConnectionIds = visitorSession.ConnectionIds,
                    VisitorIdentity = visitorSession.SessionId,
                    OperatorName = operatorSession.Name,
                    OperatorConnectionIds = operatorSession.ConnectionIds
                };
            }

            return null;
        }

        public AvailableOperatorResponse GetAvailableOperator(string accountKey)
        {
            var operatorSession = _chatSession.GetFirstAvailableOperator(accountKey);

            if (operatorSession != null)
            {
                return new AvailableOperatorResponse
                {
                    OperatorId = operatorSession.Id,
                    OperatorName = operatorSession.Name,
                    OperatorConnectionIds = operatorSession.ConnectionIds
                };
            }

            return null;
        }

        public async Task<ReturningVisitorResponse> GetReturningVisitorAsync(string accountKey, string visitorIdentity)
        {
            if (string.IsNullOrWhiteSpace(visitorIdentity) || string.IsNullOrWhiteSpace(accountKey))
            {
                return null;
            }

            var visitor = await _context.Visitors.Where(v => v.Identifier == visitorIdentity && v.Account.Key == accountKey).SingleOrDefaultAsync();
            if (visitor != null)
            {
                return new ReturningVisitorResponse
                {
                    VisitorName = visitor.Name,
                    VisitorEmail = visitor.Email
                };
            }

            return null;
        }
    }
}