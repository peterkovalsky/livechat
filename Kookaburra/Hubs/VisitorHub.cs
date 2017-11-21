using AutoMapper;
using Hangfire;
using Kookaburra.Common;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
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
        private readonly IVisitorChatService _visitorChatService;
        private readonly IVisitorService _visitorService;
        private readonly IOfflineMessageService _offlineMessageService;

        public VisitorHub(IVisitorChatService visitorChatService, IVisitorService visitorService, IOfflineMessageService offlineMessageService)
        {            
            _visitorChatService = visitorChatService;         
            _visitorService = visitorService;
            _offlineMessageService = offlineMessageService;
        }


        public async Task<InitWidgetViewModel> InitWidget(string accountKey)
        {            
            var operatorResult = _visitorChatService.GetAvailableOperator(accountKey);

            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorIdentity = visitorCookie.GetVisitorKey(accountKey);

            // returning visitor
            if (!string.IsNullOrWhiteSpace(visitorIdentity))
            {                
                var returningVisitor = await _visitorChatService.GetReturningVisitorAsync(accountKey, visitorIdentity);

                if (operatorResult != null) // Is there any available operator
                {                
                    var resumedConversation = await _visitorChatService.ResumeVisitorChatAsync(visitorIdentity, Context.ConnectionId);
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
                visitorIdentity = visitorCookie.GenerateVisitorKey();

                var visitor = await _visitorService.AddNewVisitorAsync(accountKey, visitorIdentity, WebHelper.GetIPAddress());

                // update geo details in the background
                BackgroundJob.Enqueue<IVisitorService>(visitorService => visitorService.UpdateVisitorGeolocationAsync(visitorIdentity));

                // introduction screen
                if (operatorResult != null)
                {
                    return new InitWidgetViewModel(WidgetStepType.Introduction)
                    {
                        CookieName = visitorCookie.GetCookieName(accountKey),
                        NewVisitorKey = visitorIdentity
                    };
                }

                // offline message
                return new InitWidgetViewModel(WidgetStepType.Offline)
                {
                    CookieName = visitorCookie.GetCookieName(accountKey),
                    NewVisitorKey = visitorIdentity
                };
            }
        }

        public async Task<StartChatViewModel> StartChat(IntroductionViewModel visitor)
        {            
            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorIdentity = visitorCookie.GetOrCreateVisitorKey(visitor.AccountKey);

            var availableOperator = _visitorChatService.GetAvailableOperator(visitor.AccountKey);
            ResumeVisitorChatResponse resumedConversation = null;

            // if operator is available - establish connection
            if (availableOperator != null)
            {
                await _visitorChatService.VisitorStartChatAsync(new VisitorStartChatRequest
                {
                    OperatorId = availableOperator.OperatorId,
                    VisitorKey = visitorIdentity,
                    VisitorName = visitor.Name,
                    VisitorEmail = visitor.Email,
                    VisitorIP = WebHelper.GetIPAddress(),
                    Page = visitor.PageUrl,
                    AccountKey = visitor.AccountKey               
                });

                // update geo details in the background
                BackgroundJob.Enqueue<IVisitorService>(visitorService => visitorService.UpdateVisitorGeolocationAsync(visitorIdentity));                

                resumedConversation = await _visitorChatService.ResumeVisitorChatAsync(visitorIdentity, Context.ConnectionId);
                if (resumedConversation != null)
                {
                    var visitorInfo = new OperatorConversationViewModel
                    {
                        SessionId = visitorIdentity,
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
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnectedGlobal(visitorIdentity);
                    Clients.Clients(resumedConversation.OperatorInfo.ConnectionIds).visitorConnected(visitorInfo);                 
                }
            }

            return new StartChatViewModel
            {
                CookieName = visitorCookie.GetCookieName(visitor.AccountKey),
                SessionId = visitorIdentity,
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
            var currentSession = _visitorChatService.GetCurrentSessionByConnection(Context.ConnectionId);

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
            Clients.Clients(currentSession.OperatorConnectionIds).sendMessageToOperator(messageView, currentSession.VisitorIdentity);            

            await _visitorChatService.VisitorMessagedAsync(Context.ConnectionId, message, dateSent);
        }

        /// <summary>
        /// Visitor wants to stop the conversation with operator
        /// </summary>
        public async Task FinishChattingWithOperator(string accountKey)
        {
            var visitorCookie = new VisitorCookie(Context.Request.GetHttpContext());
            var visitorIdentity = visitorCookie.GetVisitorKey(accountKey);
       
            var currentSession = _visitorChatService.GetCurrentSessionByIdentity(visitorIdentity);

            await _visitorChatService.StopChatAsync(visitorIdentity);

            DisconnectVisitor(currentSession, UserType.Visitor);
        }

        #region Private Methods

        /// <summary>
        /// Introduction of a new visitor
        /// </summary>        
        private InitWidgetViewModel Introduction(ReturningVisitorResponse returningVisitor)
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
        private InitWidgetViewModel Resume(ResumeVisitorChatResponse resumedConversation)
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
        private InitWidgetViewModel Offline(ReturningVisitorResponse returningVisitor)
        {
            var viewModel = new InitWidgetViewModel(WidgetStepType.Offline);

            if (returningVisitor != null)
            {
                viewModel.VisitorName = returningVisitor.VisitorName;
                viewModel.VisitorEmail = returningVisitor.VisitorEmail;
            }

            return viewModel;
        }

        private void DisconnectVisitor(CurrentSessionResponse currentSession, UserType disconnectedBy)
        {
            if (currentSession != null)
            {
                var diconnectView = new DisconnectViewModel
                {
                    VisitorSessionId = currentSession.VisitorIdentity,
                    TimeStamp = DateTime.UtcNow.JsDateTime(),
                    DisconnectedBy = disconnectedBy.ToString()
                };

                // Notify all operator instances
                Clients.Clients(currentSession.OperatorConnectionIds).visitorDisconnectedGlobal(currentSession.VisitorIdentity);
                Clients.Clients(currentSession.OperatorConnectionIds.AllBut(Context.ConnectionId)).visitorDisconnected(diconnectView);
                // Notify all visitor instances
                Clients.Clients(currentSession.VisitorConnectionIds.AllBut(Context.ConnectionId)).visitorDisconnected(diconnectView);
            }
        }

        #endregion
    }
}