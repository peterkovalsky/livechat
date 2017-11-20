using Kookaburra.Domain.Common;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public interface IChatService
    {
        Task<TranscriptResponse> GetTranscriptAsync(long conversationId, string operatorIdentity);

        Task<ChatHistoryResponse> GetChatHistoryAsync(TimeFilterType timeFilter, string operatorIdentity, Pagination pagination = null);

        Task<ChatHistoryResponse> SearchChatHistoryAsync(string query, string operatorIdentity, Pagination pagination = null);
    }
}