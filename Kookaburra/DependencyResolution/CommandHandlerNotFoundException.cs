using System;

namespace Kookaburra.DependencyResolution
{
    public class CommandHandlerNotFoundException : Exception
    {
        public CommandHandlerNotFoundException()
        {
        }

        public CommandHandlerNotFoundException(Type type)
            : base(string.Format("Could not find query handler for {0}", type.ToString()))
        {

        }

        public CommandHandlerNotFoundException(string message)
        : base(message)
        {
        }

        public CommandHandlerNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}