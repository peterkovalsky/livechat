using System;

namespace Kookaburra.Domain.Command.Model
{
    public class VisitorMessagedCommand : ICommand
    {
        public VisitorMessagedCommand(string visitorConnectionId, string operatorConnectionId, string message, DateTime dateSent)
        {
            VisitorConnectionId = visitorConnectionId;
            OperatorConnectionId = operatorConnectionId;
            Message = message;
            DateSent = dateSent;
        }

        public string VisitorConnectionId { get; private set; }

        public string OperatorConnectionId { get; private set; }

        public string Message { get; private set; }

        public DateTime DateSent { get; private set; }                
    }
}