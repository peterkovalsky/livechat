using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public interface IChatService
    {
        Task ConnectOperatorAsync(string accountKey, string operatorIdentity, string operatorConnectionId);

        Task OperatorMessagedAsync(string visitorIdentity, string message, DateTime dateSent, UserType sentBy = UserType.Operator);

        Task VisitorMessagedAsync(string visitorConnectionId, string message, DateTime dateSent);

        Task<long> VisitorStartChatAsync(VisitorStartChatRequest request);

        Task<List<Conversation>> GetVisitorQueueAsync(string operatorKey);

        Task<List<Conversation>> GetLiveChatsAsync(string operatorKey);

        Task DisconnectOperatorAsync(string connectionId);

        Task StopChatAsync(string visitorIndentity, long conversationId = default(long));
    }
}