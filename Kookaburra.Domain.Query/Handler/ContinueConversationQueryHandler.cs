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
            if (string.IsNullOrWhiteSpace(query.SessionId))
            {
                return null;
            }

            var conversation = _context.Conversations
                .Include(i => i.Messages)
                .Include(i => i.Visitor)
                .Include(i => i.Operator)
                .Where(c => c.Visitor.SessionId == query.SessionId && c.TimeFinished == null)
                .SingleOrDefault();

            if (conversation != null)
            {
                var conversationItems = conversation.Messages.Select(m => new ConversationItem
                {
                    Name = m.SentBy == UserType.Visitor.ToString() ? conversation.Visitor.Name : conversation.Operator.FirstName,
                    Message = m.Text,
                    TimeSent = m.DateSent
                }).ToList();

                return new ContinueConversationQueryResult { Conversation = conversationItems };
            }

            return null;
        }
    }
}