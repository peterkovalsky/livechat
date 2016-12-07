using System;

namespace Kookaburra.DependencyResolution
{
    public class QueryHandlerNotFoundException : Exception
    {
        public QueryHandlerNotFoundException()
        {
        }

        public QueryHandlerNotFoundException(Type type)
            : base(string.Format("Could not find query handler for {0}", type.ToString()))
        {
            
        }

        public QueryHandlerNotFoundException(string message)
        : base(message)
        {
        }

        public QueryHandlerNotFoundException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}