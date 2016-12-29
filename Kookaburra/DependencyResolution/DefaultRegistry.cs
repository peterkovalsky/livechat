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
    using Controllers;
    using Domain;
    using Domain.Command;
    using Domain.Command.Handler;
    using Domain.Command.Model;
    using Domain.Query;
    using Domain.Query.Handler;
    using Domain.Query.Model;
    using Domain.Query.Result;
    using Microsoft.AspNet.SignalR;
    using Repository;
    using StructureMap;
    using StructureMap.Graph;

    public class DefaultRegistry : Registry {
        #region Constructors and Destructors

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
            ForConcreteType<AccountController>().Configure.SelectConstructor(() => new AccountController(null));
            For<IDependencyResolver>().Add<StructureMapSignalRDependencyResolver>();

            // Commands
            For<ICommandHandler<StartConversationCommand>>().Add<StartConversationCommandHandler>();
            For<ICommandHandler<ConnectOperatorCommand>>().Add<ConnectOperatorCommandHandler>();
            For<ICommandHandler<LeaveMessageCommand>>().Add<LeaveMessageCommandHandler>();
            For<ICommandHandler<OperatorMessagedCommand>>().Add<OperatorMessagedCommandHandler>();
            For<ICommandHandler<StopConversationCommand>>().Add<StopConversationCommandHandler>();
            For<ICommandHandler<VisitorMessagedCommand>>().Add<VisitorMessagedCommandHandler>();

            // Queries
            For<IQueryHandler<AvailableOperatorQuery, AvailableOperatorQueryResult>>().Add<AvailableOperatorQueryHandler>();
            For<IQueryHandler<VisitorSessionQuery, VisitorSessionQueryResult>>().Add<VisitorSessionQueryHandler>();
            For<IQueryHandler<ContinueConversationQuery, ContinueConversationQueryResult>>().Add<ContinueConversationQueryHandler>();
            
            ForSingletonOf<ChatSession>();
        }

        #endregion
    }
}