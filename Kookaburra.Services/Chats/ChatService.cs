using Kookaburra.Domain;
using Kookaburra.Domain.Common;
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


        public ChatService(KookaburraContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }


        public async Task<List<ChatsPerDayResponse>> GetChatsPerDay(string operatorIdentity, int lastNumberOfDays)
        {
            var chatsInPeriod = new List<ChatsPerDayResponse>();

            var account = await _accountService.GetAccountForOperatorAsync(operatorIdentity);
            var dateStart = DateTime.UtcNow.AddDays(-lastNumberOfDays);

            var chatsPerDay = await (from chat in _context.Conversations
                          where chat.TimeStarted >= dateStart &&
                                chat.Operator.Account.Key == account.Key
                          let dt = DbFunctions.TruncateTime(chat.TimeStarted)
                          group chat by dt into g
                          select new ChatsPerDayResponse
                          {
                              Day = g.Key.Value,
                              TotalChats = g.Count()
                          })
                          .ToListAsync();

            for(int i=0; i<lastNumberOfDays; i++)
            {
                var now = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                var day = now.AddDays(-i);
                var chats = chatsPerDay.FirstOrDefault(c => c.Day == day);

                if (chats == null)
                {
                    chats = new ChatsPerDayResponse { Day = day, TotalChats = 0 };
                }

                chatsInPeriod.Add(chats);
            }

            return chatsInPeriod.OrderBy(c => c.Day).ToList();
        }

        public async Task<TranscriptResponse> GetTranscriptAsync(long conversationId, string operatorIdentity)
        {
            var account = await _accountService.GetAccountForOperatorAsync(operatorIdentity);

            var result = await _context.Conversations
            .Where(c => c.Id == conversationId && c.Operator.Account.Key == account.Key && c.TimeFinished != null)
            .Select(c =>
            new
            {
                TimeFinished = c.TimeFinished.Value,
                TranscriptQueryResult = new TranscriptResponse
                {
                    TimeStarted = c.TimeStarted,
                    Visitor = new VisitorResponse
                    {
                        Name = c.Visitor.Name,
                        Email = c.Visitor.Email,
                        Country = c.Visitor.Country,
                        CountryCode = c.Visitor.CountryCode,
                        City = c.Visitor.City,
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
                }
            }).SingleOrDefaultAsync();

            if (result == null) return null;

            var chatDuration = result.TimeFinished - result.TranscriptQueryResult.TimeStarted;

            result.TranscriptQueryResult.Duration = new Duration((int)chatDuration.TotalMinutes, (int)chatDuration.TotalSeconds);

            return result.TranscriptQueryResult;
        }

        public async Task<ChatHistoryResponse> GetChatHistoryAsync(TimeFilterType timeFilter, string operatorIdentity, Pagination pagination = null)
        {
            var account = await _accountService.GetAccountForOperatorAsync(operatorIdentity);

            var conversations = _context.Conversations.Where(c =>
                               c.Operator.Account.Key == account.Key
                               && c.Messages.Any(m => m.SentBy == UserType.Visitor.ToString())
                               && c.TimeFinished != null);

            if (timeFilter == TimeFilterType.Today)
            {
                var aDayAgo = DateTime.UtcNow.AddDays(-1);

                conversations = conversations.Where(c => aDayAgo <= c.TimeStarted);
            }
            else if (timeFilter == TimeFilterType.Week)
            {
                var aWeekAgo = DateTime.UtcNow.AddDays(-7);

                conversations = conversations.Where(c => aWeekAgo <= c.TimeStarted);
            }
            else if (timeFilter == TimeFilterType.Fortnight)
            {
                var aFortnightAgo = DateTime.UtcNow.AddDays(-14);

                conversations = conversations.Where(c => aFortnightAgo <= c.TimeStarted);
            }
            else if (timeFilter == TimeFilterType.Month)
            {
                var aMonthAgo = DateTime.UtcNow.AddMonths(-1);

                conversations = conversations.Where(c => aMonthAgo <= c.TimeStarted);
            }
            else if (timeFilter == TimeFilterType.Year)
            {
                var aYearAgo = DateTime.UtcNow.AddYears(-1);

                conversations = conversations.Where(c => aYearAgo <= c.TimeStarted);
            }
            else if (timeFilter == TimeFilterType.All)
            {
            }

            var total = await conversations.CountAsync();

            if (pagination != null)
            {
                conversations = conversations.OrderByDescending(om => om.TimeStarted).Skip(pagination.Skip).Take(pagination.Size);
            }

            return new ChatHistoryResponse
            {
                TotalConversations = total,

                Conversations = await conversations.Select(c => new ConversationItemResponse
                {
                    Id = c.Id,
                    VisitorName = c.Visitor.Name,
                    OperatorName = c.Operator.FirstName,
                    Text = c.Messages.FirstOrDefault(m => m.SentBy == UserType.Visitor.ToString()).Text,
                    StartTime = c.TimeStarted,
                    TotalMessages = c.Messages.Count()
                }).ToListAsync()
            };
        }

        public async Task<ChatHistoryResponse> SearchChatHistoryAsync(string query, string operatorIdentity, Pagination pagination = null)
        {
            var account = await _accountService.GetAccountForOperatorAsync(operatorIdentity);

            var conversations = _context.Conversations.Where(c =>
                               c.Operator.Account.Key == account.Key
                               && c.Messages.Any(m => m.SentBy == UserType.Visitor.ToString())
                               && c.TimeFinished != null);

            conversations = conversations.Where(c =>
                                   c.Messages.Any(m => m.Text.Contains(query))
                                || c.Visitor.Name.Contains(query));

            var total = await conversations.CountAsync();

            if (pagination != null)
            {
                conversations = conversations.OrderByDescending(om => om.TimeStarted).Skip(pagination.Skip).Take(pagination.Size);
            }

            return new ChatHistoryResponse
            {
                TotalConversations = total,

                Conversations = await conversations.Select(c => new ConversationItemResponse
                {
                    Id = c.Id,
                    VisitorName = c.Visitor.Name,
                    OperatorName = c.Operator.FirstName,
                    Text = c.Messages.FirstOrDefault(m => m.SentBy == UserType.Visitor.ToString()).Text,
                    StartTime = c.TimeStarted,
                    TotalMessages = c.Messages.Count()
                }).ToListAsync()
            };
        }

        public async Task<List<TimmedOutConversationResponse>> TimmedOutConversationsAsync(int timeoutInMinutes)
        {
            var cutOffTime = DateTime.UtcNow.AddMinutes(-timeoutInMinutes);

            var conversations = await _context.Conversations
                .Include(i => i.Visitor)
                .Where(c => c.TimeFinished == null && c.Messages.Any() && c.Messages.OrderByDescending(m => m.DateSent).FirstOrDefault().DateSent < cutOffTime)
                .Select(c => new TimmedOutConversationResponse
                {
                    VisitorIdentity = c.Visitor.Identifier,
                    ConversationId = c.Id
                })
                .ToListAsync();

            return conversations;
        }
    }
}