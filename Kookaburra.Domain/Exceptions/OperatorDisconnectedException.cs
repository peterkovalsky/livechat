using System;

namespace Kookaburra.Exceptions
{
    public class OperatorDisconnectedException : Exception
    {
        public OperatorDisconnectedException()
        {
        }

        public OperatorDisconnectedException(string message)
            : base(message)
        {
        }

        public OperatorDisconnectedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}