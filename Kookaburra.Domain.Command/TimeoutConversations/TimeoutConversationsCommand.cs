using System;

namespace Kookaburra.Domain.Command.TimeoutConversations
{
    public class TimeoutConversationsCommand : ICommand
    {
        public TimeoutConversationsCommand(int timeoutInMinutes)
        {
            TimeoutInMinutes = timeoutInMinutes;
        }

        public int TimeoutInMinutes { get; }

        public string OperatorIdentity => throw new NotImplementedException();
    }
}