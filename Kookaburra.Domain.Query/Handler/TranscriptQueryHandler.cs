﻿using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Repository;
using System.Linq;

namespace Kookaburra.Domain.Query.Handler
{
    public class TranscriptQueryHandler : IQueryHandler<TranscriptQuery, TranscriptQueryResult>
    {
        private readonly KookaburraContext _context;

        public TranscriptQueryHandler(KookaburraContext context)
        {
            _context = context;
        }

        public TranscriptQueryResult Execute(TranscriptQuery query)
        {
            var account = _context.Accounts.Where(a => a.Operators.Any(o => o.Identity == query.OperatorIdentity)).SingleOrDefault();

            return _context.Conversations
                .Where(c => c.Id == query.ConversationId && c.Operator.AccountId == account.Id && c.TimeFinished != null)
                .Select(c => new TranscriptQueryResult
                {
                    TimeStarted = c.TimeStarted,
                    TimeFinished = c.TimeFinished.Value,
                    Visitor = new VisitorResult
                    {
                        Name = c.Visitor.Name,
                        Email = c.Visitor.Email,
                        Country = c.Visitor.Country,
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
                }).SingleOrDefault();                
        }
    }
}