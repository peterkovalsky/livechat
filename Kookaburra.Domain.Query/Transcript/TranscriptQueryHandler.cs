using Kookaburra.Domain.Common;
using Kookaburra.Domain.ResumeVisitorChat;
using Kookaburra.Repository;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.Transcript
{
    public class TranscriptQueryHandler : IQueryHandler<TranscriptQuery, Task<TranscriptQueryResult>>
    {
        private readonly KookaburraContext _context;

        public TranscriptQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public async Task<TranscriptQueryResult> ExecuteAsync(TranscriptQuery query)
        {
            var result = await _context.Conversations
                .Where(c => c.Id == query.ConversationId && c.Operator.Account.Identifier == query.AccountKey && c.TimeFinished != null)
                .Select(c =>
                new
                {
                    TimeFinished = c.TimeFinished.Value,
                    TranscriptQueryResult = new TranscriptQueryResult
                    {
                        TimeStarted = c.TimeStarted,
                        Visitor = new VisitorResult
                        {
                            Name = c.Visitor.Name,
                            Email = c.Visitor.Email,
                            Country = c.Visitor.Country,
                            CountryCode = c.Visitor.CountryCode,
                            City = c.Visitor.City,
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
                    }
                }).SingleOrDefaultAsync();

            if (result == null) return null;

            var chatDuration = result.TimeFinished - result.TranscriptQueryResult.TimeStarted;

            result.TranscriptQueryResult.Duration = new Duration((int)chatDuration.TotalMinutes, (int)chatDuration.TotalSeconds);

            return result.TranscriptQueryResult;
        }
    }
}