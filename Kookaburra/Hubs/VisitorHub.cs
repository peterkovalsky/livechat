using AutoMapper;
using Hangfire;
using Kookaburra.Common;
using Kookaburra.Domain.AvailableOperator;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.CurrentSession;
using Kookaburra.Domain.Query.ReturningVisitor;
using Kookaburra.Domain.ResumeVisitorChat;
using Kookaburra.Models;
using Kookaburra.Models.Chat;
using Kookaburra.Models.Widget;
using Kookaburra.Services.Chats;
using Kookaburra.Services.OfflineMessages;
using Kookaburra.Services.Visitors;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kookaburra.Services
{
    public class VisitorHub : Hub
    {
        private readonly IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> _currentSessionQueryHandler;
        private readonly IQueryHandler<AvailableOperatorQuery, Task<AvailableOperatorQueryResult>> _availableOperatorQueryHandler;
        private readonly IQueryHandler<ResumeVisitorChatQuery, Task<ResumeVisitorChatQueryResult>> _resumeVisitorChatQueryHandler;
        private readonly IQueryHandler<ReturningVisitorQuery, Task<ReturningVisitorQueryResult>> _returningVisitorQueryHandler;

        private readonly IChatService _chatService;
        private readonly IVisitorService _visitorService;
        private readonly IOfflineMessageService _offlineMessageService;

        public VisitorHub(
            IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>> currentSessionQueryHandler,
            IQueryHandler<AvailableOperatorQuery, Task<AvailableOperatorQueryResult>> availableOperatorQueryHandler,
            IQueryHandler<ResumeVisitorChatQuery, Task<ResumeVisitorChatQueryResult>> resumeVisitorChatQueryHandler,
            IQueryHandler<ReturningVisitorQuery, Task<ReturningVisitorQueryResult>> returningVisitorQueryHandler,
            IChatService chatService,
            IVisitorService visitorService,
            IOfflineMessageService offlineMessageService)
        {            
            _currentSessionQueryHandler = currentSessionQueryHandler;
            _availableOperatorQueryHandler = availableOperatorQueryHandler;
            _resumeVisitorChatQueryHandler = resumeVisitorChatQueryHandler;
            _returningVisitorQueryHandler = returningVisitorQueryHandler;

            _chatService = chatService;
            _visitorService = visitorService;
            _offlineMessageService = offlineMessageService;
        }


        public async Task<InitWidgetViewModel> InitWidget(string accountKey)
        {
            var query = new AvailableOperatorQuery(accountKey);
            var operatorResult = await _availableOperatorQueryHandler.ExecuteAsync(query);

            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorKey = visitorCookie.GetVisitorKey(accountKey);

            // returning visitor
            if (!string.IsNullOrWhiteSpace(visitorKey))
            {
                var queryReturning = new ReturningVisitorQuery(accountKey, visitorKey);
                var returningVisitor = await _returningVisitorQueryHandler.ExecuteAsync(queryReturning);

                if (operatorResult != null) // Is there any available operator
                {
                    var queryResume = new ResumeVisitorChatQuery(visitorKey, Context.ConnectionId);
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
                visitorKey = visitorCookie.GenerateVisitorKey();

                var visitor = await _visitorService.AddNewVisitorAsync(accountKey, visitorKey, WebHelper.GetIPAddress());

                // update geo details in the background
                BackgroundJob.Enqueue<IVisitorService>(visitorService => visitorService.UpdateVisitorGeolocationAsync(visitorKey));

                // introduction screen
                if (operatorResult != null)
                {
                    return new InitWidgetViewModel(WidgetStepType.Introduction)
                    {
                        CookieName = visitorCookie.GetCookieName(accountKey),
                        NewVisitorKey = visitorKey
                    };
                }

                // offline message
                return new InitWidgetViewModel(WidgetStepType.Offline)
                {
                    CookieName = visitorCookie.GetCookieName(accountKey),
                    NewVisitorKey = visitorKey
                };
            }
        }

        public async Task<StartChatViewModel> StartChat(IntroductionViewModel visitor)
        {            
            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorKey = visitorCookie.GetOrCreateVisitorKey(visitor.AccountKey);

            var query = new AvailableOperatorQuery(visitor.AccountKey);
            var availableOperator = await _availableOperatorQueryHandler.ExecuteAsync(query);

            ResumeVisitorChatQueryResult resumedConversation = null;

            // if operator is available - establish connection
            if (availableOperator != null)
            {
                await _chatService.VisitorStartChatAsync(new VisitorStartChatRequest
                {
                    OperatorId = availableOperator.OperatorId,
                    VisitorKey = visitorKey,
                    VisitorName = visitor.Name,
                    VisitorEmail = visitor.Email,
                    VisitorIP = WebHelper.GetIPAddress(),
                    Page = visitor.PageUrl,
                    AccountKey = visitor.AccountKey               
                });

                // update geo details in the background
                BackgroundJob.Enqueue<IVisitorService>(visitorService => visitorService.UpdateVisitorGeolocationAsync(visitorKey));


                var queryResume = new ResumeVisitorChatQuery(visitorKey, Context.ConnectionId);
                resumedConversation = await _resumeVisitorChatQueryHandler.ExecuteAsync(queryResume);

                if (resumedConversation != null)
                {
                    var visitorInfo = new OperatorConversationViewModel
                    {
                        SessionId = visitorKey,
                        VisitorName = visitor.Name,
                        Email = visitor.Email,
                        Country = resumedConversation.VisitorInfo.Country,
                        City = resumedConversation.VisitorInfo.City,
                        Region = resumedConversation.VisitorInfo.Region,
                        CurrentUrl = visitor.PageUrl,
                        StartTime = DateTime.UtcNow.JsDateTime(),
                        Messages = Mapper.Map<List<MessageViewModel>>(resumedConversation.Conversation)
                    };

                    // Notify all operator instances about this visitor
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnectedGlobal(visitorKey);
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnected(visitorInfo);                 
                }
            }

            return new StartChatViewModel
            {
                CookieName = visitorCookie.GetCookieName(visitor.AccountKey),
                SessionId = visitorKey,
                OperatorName = availableOperator?.OperatorName,
                Messages = resumedConversation != null ? Mapper.Map<List<MessageViewModel>>(resumedConversation.Conversation) : null
            };
        }

        public async Task SendOfflineMessage(OfflineViewModel messageViewModel)
        {
            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorKey = visitorCookie.GetOrCreateVisitorKey(messageViewModel.AccountKey);         

            var offlineMessage = Mapper.Map<OfflineMessage>(messageViewModel);
            var visitor = Mapper.Map<Visitor>(messageViewModel);

            visitor.IpAddress = WebHelper.GetIPAddress();
            visitor.Identifier = visitorKey;

            var messageId = await _offlineMessageService.LeaveMessage(offlineMessage, visitor, messageViewModel.AccountKey);

            // send email notification to operators in the background
            BackgroundJob.Enqueue<IEmailService>(emailService => emailService.SendOfflineNotificationEmail(messageId));

            // update geo details in the background
            BackgroundJob.Enqueue<IVisitorService>(visitorService => visitorService.UpdateVisitorGeolocationAsync(visitorKey));
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

            await _chatService.VisitorMessagedAsync(Context.ConnectionId, message, dateSent);
        }

        /// <summary>
        /// Visitor wants to stop the conversation with operator
        /// </summary>
        public async Task FinishChattingWithOperator(string accountKey)
        {
            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorKey = visitorCookie.GetVisitorKey(accountKey);

            var query = new CurrentSessionQuery
            {
                VisitorSessionId = visitorKey
            };
            var currentSession = await _currentSessionQueryHandler.ExecuteAsync(query);            

            await _chatService.StopChatAsync(visitorKey);

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