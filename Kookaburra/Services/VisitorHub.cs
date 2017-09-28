using AutoMapper;
using Hangfire;
using Kookaburra.Common;
using Kookaburra.Domain.AvailableOperator;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.LeaveMessage;
using Kookaburra.Domain.Command.StartVisitorChat;
using Kookaburra.Domain.Command.StopConversation;
using Kookaburra.Domain.Command.VisitorMessaged;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.CurrentSession;
using Kookaburra.Domain.Query.ReturningVisitor;
using Kookaburra.Domain.ResumeVisitorChat;
using Kookaburra.Models;
using Kookaburra.Models.Chat;
using Kookaburra.Models.Widget;
using Kookaburra.Services.Visitors;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Services
{
    public class VisitorHub : Hub
    {
        private readonly ICommandHandler<StopConversationCommand> _stopConversationCommandHandler;
        private readonly ICommandHandler<StartVisitorChatCommand> _startVisitorChatCommandHandler;
        private readonly ICommandHandler<VisitorMessagedCommand> _visitorMessagedCommandHandler;
        private readonly ICommandHandler<LeaveMessageCommand> _leaveMessageCommandHandler;

        private readonly IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> _currentSessionQueryHandler;
        private readonly IQueryHandler<AvailableOperatorQuery, Task<AvailableOperatorQueryResult>> _availableOperatorQueryHandler;
        private readonly IQueryHandler<ResumeVisitorChatQuery, Task<ResumeVisitorChatQueryResult>> _resumeVisitorChatQueryHandler;
        private readonly IQueryHandler<ReturningVisitorQuery, Task<ReturningVisitorQueryResult>> _returningVisitorQueryHandler;

        private readonly IVisitorService _visitorService;

        public VisitorHub(
            ICommandHandler<StopConversationCommand> stopConversationCommandHandler,
            ICommandHandler<StartVisitorChatCommand> startVisitorChatCommandHandler,
            ICommandHandler<VisitorMessagedCommand> visitorMessagedCommandHandler,
            ICommandHandler<LeaveMessageCommand> leaveMessageCommandHandler,
            IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> currentSessionQueryHandler,
            IQueryHandler<AvailableOperatorQuery, Task<AvailableOperatorQueryResult>> availableOperatorQueryHandler,
            IQueryHandler<ResumeVisitorChatQuery, Task<ResumeVisitorChatQueryResult>> resumeVisitorChatQueryHandler,
            IQueryHandler<ReturningVisitorQuery, Task<ReturningVisitorQueryResult>> returningVisitorQueryHandler,
            IVisitorService visitorService)
        {
            _stopConversationCommandHandler = stopConversationCommandHandler;
            _startVisitorChatCommandHandler = startVisitorChatCommandHandler;
            _visitorMessagedCommandHandler = visitorMessagedCommandHandler;
            _leaveMessageCommandHandler = leaveMessageCommandHandler;

            _currentSessionQueryHandler = currentSessionQueryHandler;
            _availableOperatorQueryHandler = availableOperatorQueryHandler;
            _resumeVisitorChatQueryHandler = resumeVisitorChatQueryHandler;
            _returningVisitorQueryHandler = returningVisitorQueryHandler;

            _visitorService = visitorService;
        }


        public async Task<InitWidgetViewModel> InitWidget(string accountKey)
        {
            var query = new AvailableOperatorQuery(accountKey);
            var operatorResult = await _availableOperatorQueryHandler.ExecuteAsync(query);

            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorId = visitorCookie.GetVisitorId(accountKey);

            // returning visitor
            if (!string.IsNullOrWhiteSpace(visitorId))
            {
                var queryReturning = new ReturningVisitorQuery(accountKey, visitorId);
                var returningVisitor = await _returningVisitorQueryHandler.ExecuteAsync(queryReturning);

                if (operatorResult != null) // Is there any available operator
                {
                    var queryResume = new ResumeVisitorChatQuery(visitorId, Context.ConnectionId);
                    var resumedConversation = await _resumeVisitorChatQueryHandler.ExecuteAsync(queryResume);
                    if (resumedConversation != null) // check if session is still alive
                    {
                        // resume chat session
                        return Resume(resumedConversation);
                    }

                    // new session                 
                    return Introduction(returningVisitor);
                }

                // no operators available
                return Offline(returningVisitor);
            }
            else // new visitor
            {
                visitorId = visitorCookie.GenerateVisitorId();

                await _visitorService.AddNewVisitorAsync(accountKey, visitorId, WebHelper.GetIPAddress());

                if (operatorResult != null)
                {
                    return new InitWidgetViewModel(WidgetStepType.Introduction)
                    {
                        CookieName = visitorCookie.GetCookieName(accountKey),
                        NewVisitorId = visitorId
                    };
                }

                return new InitWidgetViewModel(WidgetStepType.Offline)
                {
                    CookieName = visitorCookie.GetCookieName(accountKey),
                    NewVisitorId = visitorId
                };
            }
        }

        public async Task<StartChatViewModel> StartChat(IntroductionViewModel visitor)
        {            
            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorId = visitorCookie.GetOrCreateVisitorId(visitor.AccountKey);

            var query = new AvailableOperatorQuery(visitor.AccountKey);
            var availableOperator = await _availableOperatorQueryHandler.ExecuteAsync(query);

            ResumeVisitorChatQueryResult resumedConversation = null;

            // if operator is available - establish connection
            if (availableOperator != null)
            {              
                var command = new StartVisitorChatCommand(availableOperator.OperatorId, visitor.Name, visitorId, visitor.AccountKey)
                {
                    Page = visitor.PageUrl,
                    VisitorIP = WebHelper.GetIPAddress(),
                    VisitorEmail = visitor.Email
                };
                await _startVisitorChatCommandHandler.ExecuteAsync(command);


                var queryResume = new ResumeVisitorChatQuery(visitorId, Context.ConnectionId);
                resumedConversation = await _resumeVisitorChatQueryHandler.ExecuteAsync(queryResume);

                if (resumedConversation != null)
                {
                    var visitorInfo = new OperatorConversationViewModel
                    {
                        SessionId = visitorId,
                        VisitorName = visitor.Name,
                        Email = visitor.Email,
                        Country = resumedConversation.VisitorInfo.Country,
                        City = resumedConversation.VisitorInfo.City,
                        Region = resumedConversation.VisitorInfo.Region,
                        CurrentUrl = visitor.PageUrl,
                        StartTime = DateTime.UtcNow.JsDateTime()
                    };

                    // Notify all operator instances about this visitor
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnectedGlobal(visitorId);
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnected(visitorInfo);                 
                }
            }

            return new StartChatViewModel
            {
                CookieName = visitorCookie.GetCookieName(visitor.AccountKey),
                SessionId = visitorId,
                OperatorName = availableOperator?.OperatorName,
                Messages = resumedConversation != null ? Mapper.Map<List<MessageViewModel>>(resumedConversation.Conversation) : null
            };
        }

        public async Task SendOfflineMessage(OfflineViewModel offlineMessage)
        {
            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorId = visitorCookie.GetOrCreateVisitorId(offlineMessage.AccountKey);

            var command = new LeaveMessageCommand(offlineMessage.AccountKey, offlineMessage.Name, offlineMessage.Email, offlineMessage.Message, AppSettings.UrlPortal)
            {
                VisitorIP = WebHelper.GetIPAddress(),
                Page = offlineMessage.Page,
                VisitorId = visitorId
            };

            await _leaveMessageCommandHandler.ExecuteAsync(command);

            BackgroundJob.Enqueue<IEmailService>(emailService => emailService.SendOfflineNotificationEmail(command.Id));            
        }

        /// <summary>
        /// Message from VISITOR to OPERATOR
        /// </summary>
        public async Task SendToOperator(string message)
        {
            var dateSent = DateTime.UtcNow;

            var query = new CurrentSessionQuery
            {
                VisitorConnectionId = Context.ConnectionId
            };
            var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);

            var messageView = new MessageViewModel
            {
                Author = currentSession.VisitorName,
                Text = message,
                SentBy = UserType.Visitor,
                SentOn = dateSent
            };

            // Notify all visitor instances (mutiple tabs)
            Clients.Clients(currentSession.VisitorConnectionIds).sendMessageToVisitor(messageView);
            // Notify all operator instances (mutiple tabs)   
            Clients.Clients(currentSession.OperatorConnectionIds).sendMessageToOperator(messageView, currentSession.VisitorSessionId);

            await _visitorMessagedCommandHandler.ExecuteAsync(new VisitorMessagedCommand(Context.ConnectionId, message, dateSent));
        }

        /// <summary>
        /// Visitor wants to stop the conversation with operator
        /// </summary>
        public async Task FinishChattingWithOperator(string accountKey)
        {
            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorId = visitorCookie.GetVisitorId(accountKey);

            var query = new CurrentSessionQuery
            {
                VisitorSessionId = visitorId
            };
            var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);

            await _stopConversationCommandHandler.ExecuteAsync(new StopConversationCommand(visitorId));

            DisconnectVisitor(currentSession, UserType.Visitor);
        }

        #region Private Methods

        /// <summary>
        /// Introduction of a new visitor
        /// </summary>        
        private InitWidgetViewModel Introduction(ReturningVisitorQueryResult returningVisitor)
        {
            var viewModel = new InitWidgetViewModel(WidgetStepType.Introduction);
            if (returningVisitor != null)
            {
                viewModel.VisitorName = returningVisitor.VisitorName;
                viewModel.VisitorEmail = returningVisitor.VisitorEmail;
            }

            return viewModel;
        }

        /// <summary>
        /// Resume chat session from where it was left off
        /// </summary> 
        private InitWidgetViewModel Resume(ResumeVisitorChatQueryResult resumedConversation)
        {
            return new InitWidgetViewModel(WidgetStepType.Resume)
            {
                VisitorName = resumedConversation.VisitorInfo.Name,
                VisitorEmail = resumedConversation.VisitorInfo.Email,
                ResumedChat = Mapper.Map<ConversationViewModel>(resumedConversation)
            };
        }

        /// <summary>
        /// Operator(s) offline - leave a message
        /// </summary>        
        private InitWidgetViewModel Offline(ReturningVisitorQueryResult returningVisitor)
        {
            var viewModel = new InitWidgetViewModel(WidgetStepType.Offline);

            if (returningVisitor != null)
            {
                viewModel.VisitorName = returningVisitor.VisitorName;
                viewModel.VisitorEmail = returningVisitor.VisitorEmail;
            }

            return viewModel;
        }

        private void DisconnectVisitor(CurrentSessionQueryResult currentSession, UserType disconnectedBy)
        {
            if (currentSession != null)
            {
                var diconnectView = new DisconnectViewModel
                {
                    VisitorSessionId = currentSession.VisitorSessionId,
                    TimeStamp = DateTime.UtcNow.JsDateTime(),
                    DisconnectedBy = disconnectedBy.ToString()
                };

                // Notify all operator instances
                Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(currentSession.VisitorSessionId);
                Clients.Clients(currentSession.OperatorConnectionIds.AllBut(Context.ConnectionId)).visitorDisconnected(diconnectView);
                // Notify all visitor instances
                Clients.Clients(currentSession.VisitorConnectionIds.AllBut(Context.ConnectionId)).visitorDisconnected(diconnectView);
            }
        }

        #endregion
    }
}