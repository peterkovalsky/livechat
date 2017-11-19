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
    using Domain.Integration;
    using Domain.Query;
    using Integration.freegeoip;
    using Kookaburra.Domain.AvailableOperator;
    using Kookaburra.Domain.Query.ChatHistory;
    using Kookaburra.Domain.Query.ChatHistorySearch;   
    using Kookaburra.Domain.Query.CurrentSession;
    using Kookaburra.Domain.Query.ReturningVisitor;
    using Kookaburra.Domain.Query.TimmedOutConversations;
    using Kookaburra.Domain.Query.Transcript; 
    using Kookaburra.Email;
    using Microsoft.AspNet.SignalR;
    using Repository;
    using StructureMap;
    using System.Threading.Tasks;

    public class DefaultRegistry : Registry {       

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.Assembly("Kookaburra.Domain");
                    scan.Assembly("Kookaburra.Repository");
                    scan.Assembly("Kookaburra.Domain.Command");
                    scan.Assembly("Kookaburra.Domain.Query");
                    scan.Assembly("Kookaburra.Services");
                    scan.Assembly("Kookaburra.Email");
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
   
            For<KookaburraContext>().Use<KookaburraContext>().Ctor<string>().Is("name=DefaultConnection");
            //ForConcreteType<AccountController>().Configure.SelectConstructor(() => new AccountController(null));
            For<IDependencyResolver>().Add<StructureMapSignalRDependencyResolver>();       
           

            // Queries
            For<IQueryHandler<AvailableOperatorQuery, Task<AvailableOperatorQueryResult>>>().Add<AvailableOperatorQueryHandler>();               
            For<IQueryHandler<CurrentSessionQuery, Task<CurrentSessionQueryResult>>>().Add<CurrentSessionQueryHandler>();                    
            For<IQueryHandler<TranscriptQuery, Task<TranscriptQueryResult>>>().Add<TranscriptQueryHandler>();
            For<IQueryHandler<ChatHistorySearchQuery, Task<ChatHistoryQueryResult>>>().Add<ChatHistorySearchQueryHandler>();            
            For<IQueryHandler<ChatHistoryQuery, Task<ChatHistoryQueryResult>>>().Add<ChatHistoryQueryHandler>();
            For<IQueryHandler<TimmedOutConversationsQuery, Task<TimmedOutConversationsQueryResult>>>().Add<TimmedOutConversationsQueryHandler>();
            For<IQueryHandler<ReturningVisitorQuery, Task<ReturningVisitorQueryResult>>>().Add<ReturningVisitorQueryHandler>();        
            
            ForSingletonOf<ChatSession>();
            ForSingletonOf<IGeoLocator>().Add<FreegeoipLocator>();
         
            For<IEmailSender>().Add<DefaultEmailSender>().Ctor<string>("host").Is(AppSettings.EmailHost)
                                                                    .Ctor<string>("username").Is(AppSettings.EmailUsername)
                                                                    .Ctor<string>("password").Is(AppSettings.EmailPassword);            
        }       
    }
}