using System;

namespace Kookaburra.Exceptions
{
    public class VisitorDisconnectedException : Exception
    {
        public VisitorDisconnectedException()
        {
        }

        public VisitorDisconnectedException(string message)
            : base(message)
        {
        }

        public VisitorDisconnectedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}