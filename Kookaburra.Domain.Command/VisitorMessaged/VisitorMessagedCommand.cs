using System;

namespace Kookaburra.Domain.Command.VisitorMessaged
{
    public class VisitorMessagedCommand : ICommand
    {
        public VisitorMessagedCommand(string visitorConnectionId, string message, DateTime dateSent, string operatorIdentity)
        {
            VisitorConnectionId = visitorConnectionId;            
            Message = message;
            DateSent = dateSent;
            OperatorIdentity = operatorIdentity;
        }

        public string VisitorConnectionId { get; }        

        public string Message { get; }

        public DateTime DateSent { get; }

        public string OperatorIdentity { get; }
    }
}