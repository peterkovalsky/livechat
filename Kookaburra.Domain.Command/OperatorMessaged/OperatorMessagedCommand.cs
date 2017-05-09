using System;

namespace Kookaburra.Domain.Command.OperatorMessaged
{
    public class OperatorMessagedCommand : ICommand
    {
        public OperatorMessagedCommand(string visitorSessionId, string message, DateTime dateSent, string operatorIdentity)
        {
            VisitorSessionId = visitorSessionId;            
            Message = message;
            DateSent = dateSent;
            OperatorIdentity = operatorIdentity;
        }   

        public string VisitorSessionId { get; }        

        public string Message { get; }

        public DateTime DateSent { get; }

        public string OperatorIdentity { get; }
    }
}