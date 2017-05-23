// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kookaburra.DependencyResolution
{
    using Domain;
    using Domain.Command;
    using Domain.Command.Handler;
    using Domain.Command.Model;
    using Domain.Integration;
    using Domain.Query;
    using Domain.Query.Handler;
    using Domain.Query.Model;
    using Domain.Query.Result;
    using Integration.freegeoip;
    using Kookaburra.Domain.AvailableOperator;
    using Kookaburra.Domain.Command.OperatorMessaged;
    using Kookaburra.Domain.Command.SignUp;
    using Kookaburra.Domain.Command.StartVisitorChat;
    using Kookaburra.Domain.Query.ChatHistory;
    using Kookaburra.Domain.Query.ChatHistorySearch;
    using Kookaburra.Domain.Query.OfflineMessages;
    using Kookaburra.Domain.Query.SearchOfflineMessages;
    using Kookaburra.Domain.ResumeVisitorChat;
    using Kookaburra.Email;
    using Kookaburra.Services;
    using Microsoft.AspNet.SignalR;
    using Repository;
    using StructureMap;
    using StructureMap.Graph;

    public class DefaultRegistry : Registry {       

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.Assembly("Kookaburra.Domain");
                    scan.Assembly("Kookaburra.Repository");
                    scan.Assembly("Kookaburra.Domain.Command");
                    scan.Assembly("Kookaburra.Domain.Query");
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
   
            For<KookaburraContext>().Use<KookaburraContext>().Ctor<string>().Is("name=DefaultConnection");
            //ForConcreteType<AccountController>().Configure.SelectConstructor(() => new AccountController(null));
            For<IDependencyResolver>().Add<StructureMapSignalRDependencyResolver>();

            // Commands
            For<ICommandHandler<StartVisitorChatCommand>>().Add<StartVisitorChatCommandHandler>();
            For<ICommandHandler<ConnectOperatorCommand>>().Add<ConnectOperatorCommandHandler>();
            For<ICommandHandler<LeaveMessageCommand>>().Add<LeaveMessageCommandHandler>();
            For<ICommandHandler<OperatorMessagedCommand>>().Add<OperatorMessagedCommandHandler>();
            For<ICommandHandler<StopConversationCommand>>().Add<StopConversationCommandHandler>();
            For<ICommandHandler<VisitorMessagedCommand>>().Add<VisitorMessagedCommandHandler>();
            For<ICommandHandler<MarkMessageAsReadCommand>>().Add<MarkMessageAsReadCommandHandler>();
            For<ICommandHandler<DeleteMessageCommand>>().Add<DeleteMessageCommandHandler>();
            For<ICommandHandler<SignUpCommand>>().Add<SignUpCommandHandler>();

            // Queries
            For<IQueryHandler<AvailableOperatorQuery, AvailableOperatorQueryResult>>().Add<AvailableOperatorQueryHandler>();       
            For<IQueryHandler<ResumeVisitorChatQuery, ResumeVisitorChatQueryResult>>().Add<ResumeVisitorChatQueryHandler>();
            For<IQueryHandler<CurrentSessionQuery, CurrentSessionQueryResult>>().Add<CurrentSessionQueryHandler>();
            For<IQueryHandler<CurrentChatsQuery, CurrentChatsQueryResult>>().Add<CurrentChatsQueryHandler>();
            For<IQueryHandler<ResumeOperatorQuery, ResumeOperatorQueryResult>>().Add<ResumeOperatorQueryHandler>();
            For<IQueryHandler<OfflineMessagesQuery, OfflineMessagesQueryResult>>().Add<OfflineMessagesQueryHandler>();
            For<IQueryHandler<SearchOfflineMessagesQuery, OfflineMessagesQueryResult>>().Add<SearchOfflineMessagesQueryHandler>();
            For<IQueryHandler<TranscriptQuery, TranscriptQueryResult>>().Add<TranscriptQueryHandler>();
            For<IQueryHandler<ChatHistorySearchQuery, ChatHistoryQueryResult>>().Add<ChatHistorySearchQueryHandler>();            
            For<IQueryHandler<ChatHistoryQuery, ChatHistoryQueryResult>>().Add<ChatHistoryQueryHandler>();
            

            ForSingletonOf<ChatSession>();
            ForSingletonOf<IGeoLocator>().Add<FreegeoipLocator>();
            For<IMailer>().Add<Mailer>();
            For<IEmailSender>().Add<DefaultEmailSender>().Ctor<string>("host").Is(AppSettings.EmailHost)
                                                                    .Ctor<string>("username").Is(AppSettings.EmailUsername)
                                                                    .Ctor<string>("password").Is(AppSettings.EmailPassword);
            For<EmailService>().Add<EmailService>();
        }       
    }
}