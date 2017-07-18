﻿using Kookaburra.Domain.Command;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kookaburra.DependencyResolution
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IDependencyResolver _resolver;

        public CommandDispatcher()
        {
            _resolver = DependencyResolver.Current;
        }

        public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            if (command == null)
            {
                throw new ArgumentNullException("Command doesn't have a reference to an instance of an object");
            }

            var handler = _resolver.GetService<ICommandHandler<TCommand>>();

            if (handler == null)
            {
                throw new CommandHandlerNotFoundException(typeof(TCommand));
            }

            await handler.ExecuteAsync(command);
        }
    }
}