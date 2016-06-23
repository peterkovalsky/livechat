using Microsoft.AspNet.SignalR;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kookaburra.DependencyResolution
{
    public class StructureMapSignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IContainer _container;
        public StructureMapSignalRDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            object service = null;
            //Below is a key difference between this StructureMap example, GetInstance is used for concrete classes.
            if (!serviceType.IsAbstract && !serviceType.IsInterface && serviceType.IsClass)
            {
                //If the type is a concrete type we get here...
                service = _container.GetInstance(serviceType);
            }
            else
            {
                // Non concrete resolution which uses the base dependency resolver if needed.
                service = _container.TryGetInstance(serviceType) ?? base.GetService(serviceType);
            }
            return service;
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            var objects = _container.GetAllInstances(serviceType).Cast<object>();
            return objects.Concat(base.GetServices(serviceType));
        }
    }
}