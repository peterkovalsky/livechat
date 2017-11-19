using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public interface IOperatorChatService
    {
        Task ConnectOperatorAsync(string operatorIdentity, string operatorConnectionId);

        Task<List<ConversationResponse>> ResumeOperatorAsync(string operatorIdentity);

        Task OperatorMessagedAsync(string visitorIdentity, string message, DateTime dateSent, UserType sentBy = UserType.Operator);

        Task<CurrentChatsResponse> GetCurrentChatsAsync(string operatorIdentity);

        Task<List<Conversation>> GetVisitorQueueAsync(string operatorIdentity);

        Task<List<Conversation>> GetLiveChatsAsync(string operatorIdentity);

        Task DisconnectOperatorAsync(string operatorConnectionId);
    }
}