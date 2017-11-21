using System;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public interface IVisitorChatService
    {
        Task<ResumeVisitorChatResponse> ResumeVisitorChatAsync(string visitorIdentity, string visitorConnectionId);

        Task VisitorMessagedAsync(string visitorConnectionId, string message, DateTime dateSent);

        Task<long> VisitorStartChatAsync(VisitorStartChatRequest request);

        Task StopChatAsync(string visitorIndentity, long conversationId = default(long));

        CurrentSessionResponse GetCurrentSessionByIdentity(string visitorIdentity);

        CurrentSessionResponse GetCurrentSessionByConnection(string visitorConnectionId);

        AvailableOperatorResponse GetAvailableOperator(string accountKey);

        Task<ReturningVisitorResponse> GetReturningVisitorAsync(string accountKey, string visitorIdentity);
    }
}