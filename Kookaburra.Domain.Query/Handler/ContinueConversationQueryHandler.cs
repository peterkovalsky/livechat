using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;

namespace Kookaburra.Domain.Query.Handler
{
    public class ContinueConversationQueryHandler : IQueryHandler<ContinueConversationQuery, ContinueConversationQueryResult>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public ContinueConversationQueryHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
        }

        public ContinueConversationQueryResult Execute(ContinueConversationQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.VisitorSessionId))
            {
                return null;
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
                // check if conversation still alive
                var visitorSession = _chatSession.GetVisitorByVisitorSessionId(query.VisitorSessionId);
                if (visitorSession != null)
                {
                    bool isNewConversation = visitorSession.ConnectionId == null;

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
                            Location = conversation.Visitor.Location,
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