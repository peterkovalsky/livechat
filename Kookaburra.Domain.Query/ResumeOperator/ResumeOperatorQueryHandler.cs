using Kookaburra.Domain.Common;
using Kookaburra.Domain.ResumeVisitorChat;
using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.ResumeOperator
{
    public class ResumeOperatorQueryHandler : IQueryHandler<ResumeOperatorQuery, Task<ResumeOperatorQueryResult>>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public ResumeOperatorQueryHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
            _chatSession = chatSession;
        }


        public async Task<ResumeOperatorQueryResult> ExecuteAsync(ResumeOperatorQuery query)
        {
            var operatorSession = _chatSession.GetOperatorByIdentity(query.OperatorIdentity);

            if (operatorSession != null)
            {
                //var conversation = (from c in _context.Conversations
                //                   join v in operatorSession.Visitors on c.Id equals v.ConversationId
                //                   where c.Operator.Identity == query.OperatorIdentity
                //                   select new ConversationResult
                //                   {
                //                       VisitorInfo = new VisitorInfoResult
                //                       {
                //                           SessionId = v.SessionId,
                //                           Name = c.Visitor.Name,
                //                           CurrentUrl = c.Page,
                //                           StartTime = c.TimeStarted,
                //                           Country = c.Visitor.Country,
                //                           City = c.Visitor.City,
                //                           Latitude = c.Visitor.Latitude,
                //                           Longitude = c.Visitor.Longitude
                //                       },
                //                       Messages = c.Messages.Select(m => new MessageResult
                //                       {
                //                           Author = m.SentBy == UserType.Visitor.ToString() ? c.Visitor.Name : c.Operator.FirstName,
                //                           Text = m.Text,
                //                           Time = m.DateSent,
                //                           SentBy = m.SentBy.ToLower()
                //                       }).ToList()
                //                   }).ToList();

                var liveConversations = operatorSession.Visitors.Select(v => v.ConversationId).ToList();

                var conversations = await _context.Conversations
                                            .Where(c => c.Operator.Identity == query.OperatorIdentity
                                                    && liveConversations.Contains(c.Id))
                                            .Select(c => new ConversationResult
                                            {
                                                VisitorInfo = new VisitorInfoResult
                                                {
                                                    SessionId = c.Visitor.SessionId,
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
    }
}