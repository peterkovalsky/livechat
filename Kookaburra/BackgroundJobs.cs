using Kookaburra.Common;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.StopConversation;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.CurrentSession;
using Kookaburra.Domain.Query.TimmedOutConversations;
using Kookaburra.Models;
using Kookaburra.Services;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra
{
    public class BackgroundJobs
    {
        private readonly ICommandHandler _commandDispatcher;
        private readonly IQueryHandler _queryDispatcher;

        public BackgroundJobs(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        public async Task TimeoutInactiveConversations()
        {
            var visitors = await _queryDispatcher.ExecuteAsync<TimmedOutConversationsQuery, Task<List<string>>>(new TimmedOutConversationsQuery(30));

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

            foreach (var visitorSessionId in visitors)
            {
                await _commandDispatcher.ExecuteAsync(new StopConversationCommand(visitorSessionId, null));

                var query = new CurrentSessionQuery(null)
                {
                    VisitorSessionId = visitorSessionId
                };
                var currentSession = await _queryDispatcher.ExecuteAsync<CurrentSessionQuery, Task<CurrentSessionQueryResult>>(query);

                if (currentSession != null)
                {
                    var diconnectView = new DisconnectViewModel
                    {
                        VisitorSessionId = visitorSessionId,
                        TimeStamp = DateTime.UtcNow.JsDateTime()
                    };

                    // Notify all operator instances
                    hubContext.Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(visitorSessionId);
                    hubContext.Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedByVisitor(diconnectView);

                    // Notify all visitor instances       
                    hubContext.Clients.Clients(currentSession.VisitorConnectionIds).visitorDisconnectedByVisitor();
                }
            }
        }
    }
}