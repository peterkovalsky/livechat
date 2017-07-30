﻿using Hangfire;
using Kookaburra.Common;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.StopConversation;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.CurrentSession;
using Kookaburra.Domain.Query.TimmedOutConversations;
using Kookaburra.Models;
using Kookaburra.Services;
using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace Kookaburra
{
    public class BackgroundJobs
    {
        private readonly ICommandHandler<StopConversationCommand> _stopConversationCommandHandler;
        private readonly IQueryHandler<TimmedOutConversationsQuery, Task<TimmedOutConversationsQueryResult>> _timmedOutConversationsQueryHandler;
        private readonly IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> _currentSessionQueryHandler;

        public BackgroundJobs(IQueryHandler<TimmedOutConversationsQuery, Task<TimmedOutConversationsQueryResult>> timmedOutConversationsQueryHandler,
            ICommandHandler<StopConversationCommand> stopConversationCommandHandler,
            IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> currentSessionQueryHandler)
        {
            _timmedOutConversationsQueryHandler = timmedOutConversationsQueryHandler;
            _stopConversationCommandHandler = stopConversationCommandHandler;
            _currentSessionQueryHandler = currentSessionQueryHandler;
        }

        [AutomaticRetry(Attempts = 0)]
        public async Task TimeoutInactiveConversations()
        {
            var result = await _timmedOutConversationsQueryHandler.ExecuteAsync(new TimmedOutConversationsQuery(30));            

            foreach (var conversation in result.Conversations)
            {            
                var query = new CurrentSessionQuery(null)
                {
                    VisitorSessionId = conversation.VisitorSessionId
                };
                var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);

                var stopCommand = new StopConversationCommand(conversation.VisitorSessionId, null)
                {
                    ConversationId = conversation.ConversationId
                };
                await _stopConversationCommandHandler.ExecuteAsync(stopCommand);

                DisconnectVisitor(currentSession, UserType.System);              
            }
        }

        private void DisconnectVisitor(CurrentSessionQueryResult currentSession, UserType disconnectedBy)
        {
            if (currentSession != null)
            {
                var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

                var diconnectView = new DisconnectViewModel
                {
                    VisitorSessionId = currentSession.VisitorSessionId,
                    TimeStamp = DateTime.UtcNow.JsDateTime(),
                    DisconnectedBy = disconnectedBy.ToString()
                };

                // Notify all operator instances
                hubContext.Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(currentSession.VisitorSessionId);
                hubContext.Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnected(diconnectView);
                // Notify all visitor instances
                hubContext.Clients.Clients(currentSession.VisitorConnectionIds).visitorDisconnected(diconnectView);
            }
        }
    }
}