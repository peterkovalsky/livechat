using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.ResumeVisitorChat
{
    public class ResumeVisitorChatQueryHandler : IQueryHandler<ResumeVisitorChatQuery, Task<ResumeVisitorChatQueryResult>>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;        

        public ResumeVisitorChatQueryHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;           
        }


        public async Task<ResumeVisitorChatQueryResult> ExecuteAsync(ResumeVisitorChatQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.VisitorSessionId) || string.IsNullOrWhiteSpace(query.VisitorConnectionId))
            {
                return null;
            }

            // check if conversation still alive 
            var visitorSession = _chatSession.GetVisitorByVisitorSessionId(query.VisitorSessionId);
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
                    _chatSession.UpdateVisitor(query.VisitorSessionId, query.VisitorConnectionId);

                    var conversationItems = conversation.Messages.Select(m => new MessageResult
                    {
                        Author = m.SentBy == UserType.Visitor.ToString() ? conversation.Visitor.Name : conversation.Operator.FirstName,
                        Text = m.Text,
                        SentOn = m.DateSent,
                        SentBy = m.SentBy.ToLower()
                    }).ToList();

                    var operatorSession = _chatSession.GetOperatorByVisitorSessionId(query.VisitorSessionId);

                    return new ResumeVisitorChatQueryResult
                    {                    
                        OperatorInfo = new OperatorInfo
                        {
                            Name = conversation.Operator.FirstName,
                            ConnectionIds = operatorSession.ConnectionIds
                        },
                        VisitorInfo = new VisitorInfoResult
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
    }
}