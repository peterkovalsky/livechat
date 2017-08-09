using System;

namespace Kookaburra.Domain.Command.OperatorMessaged
{
    public class OperatorMessagedCommand : ICommand
    {
        public OperatorMessagedCommand(string visitorSessionId, string message, DateTime dateSent)
        {
            VisitorSessionId = visitorSessionId;            
            Message = message;
            DateSent = dateSent;         
        }   

        public string VisitorSessionId { get; }        

        public string Message { get; }

        public DateTime DateSent { get; }

        public string AccountKey { get; }
    }
}