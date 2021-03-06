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
    using Integration.freegeoip;
    using Kookaburra.Email;
    using Microsoft.AspNet.SignalR;
    using Repository;
    using StructureMap;

    public class DefaultRegistry : Registry {       

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.TheCallingAssembly();
                    scan.Assembly("Kookaburra.Domain");
                    scan.Assembly("Kookaburra.Repository");                 
                    scan.Assembly("Kookaburra.Services");
                    scan.Assembly("Kookaburra.Email");
                    scan.WithDefaultConventions();
					scan.With(new ControllerConvention());
                });
   
            For<KookaburraContext>().Use<KookaburraContext>().Ctor<string>().Is("name=DefaultConnection");  
            For<IDependencyResolver>().Add<StructureMapSignalRDependencyResolver>();
            
            ForSingletonOf<ChatSession>();
            ForSingletonOf<IGeoLocator>().Add<FreegeoipLocator>();
         
            For<IEmailSender>().Add<DefaultEmailSender>().Ctor<string>("host").Is(AppSettings.EmailHost)
                                                                    .Ctor<string>("username").Is(AppSettings.EmailUsername)
                                                                    .Ctor<string>("password").Is(AppSettings.EmailPassword);            
        }       
    }
}