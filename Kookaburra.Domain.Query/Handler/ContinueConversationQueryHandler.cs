using Kookaburra.Domain.Command.Handler;
using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Linq;

namespace Kookaburra.Domain.Query.Handler
{
    public class ContinueConversationQueryHandler : IQueryHandler<ContinueConversationQuery, ContinueConversationQueryResult>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;
        private readonly OperatorMessagedCommandHandler _operatorMessagedHandler;

        public ContinueConversationQueryHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
            _operatorMessagedHandler = new OperatorMessagedCommandHandler(_context, _chatSession);
        }

        public ContinueConversationQueryResult Execute(ContinueConversationQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.VisitorSessionId))
            {
                return null;
            }

            // check if conversation still alive 
            var visitorSession = _chatSession.GetVisitorByVisitorSessionId(query.VisitorSessionId);
            if (visitorSession != null)
            {
                bool isNewConversation = visitorSession.ConnectionId == null;

                if (isNewConversation)
                {
                    // add greeting if needed
                    var command = new OperatorMessagedCommand(query.VisitorSessionId, DefaultSettings.CHAT_GREETING, DateTime.UtcNow);
                    _operatorMessagedHandler.Execute(command);
                }

                var conversation = _context.Conversations
                    .Include(i => i.Messages)
                    .Include(i => i.Visitor)
                    .Include(i => i.Operator)
                    .Where(c => c.Visitor.SessionId == query.VisitorSessionId && c.TimeFinished == null)
                    .SingleOrDefault();

                // check if operator still there            
                if (conversation != null)
                {
                    _chatSession.UpdateVisitor(query.VisitorSessionId, query.VisitorConnectionId);

                    var conversationItems = conversation.Messages.Select(m => new ConversationItem
                    {
                        Author = m.SentBy == UserType.Visitor.ToString() ? conversation.Visitor.Name : conversation.Operator.FirstName,
                        Text = m.Text,
                        Time = m.DateSent,
                        SentBy = m.SentBy.ToLower()
                    }).ToList();

                    var operatorSession = _chatSession.GetOperatorByVisitorSessionId(query.VisitorSessionId);

                    return new ContinueConversationQueryResult
                    {
                        IsNewConversation = isNewConversation,
                        OperatorInfo = new OperatorInfo
                        {
                            Name = conversation.Operator.FirstName,
                            ConnectionId = operatorSession.ConnectionId,
                            SessionId = operatorSession.SessionId
                        },
                        VisitorInfo = new VisitorInfo
                        {
                            Name = conversation.Visitor.Name,
                            Location = string.Format("{0}, {1}", conversation.Visitor.Country, conversation.Visitor.City),
                            Page = conversation.Page
                        },
                        Conversation = conversationItems
                    };
                }
            }

            return null;
        }
    }
}