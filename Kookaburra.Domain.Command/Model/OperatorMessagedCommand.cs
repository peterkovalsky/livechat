using System;

namespace Kookaburra.Domain.Command.Model
{
    public class OperatorMessagedCommand : ICommand
    {
        public OperatorMessagedCommand(string visitorSessionId, string message, DateTime dateSent)
        {
            VisitorSessionId = visitorSessionId;            
            Message = message;
            DateSent = dateSent;
        }   

        public string VisitorSessionId { get; private set; }        

        public string Message { get; private set; }

        public DateTime DateSent { get; private set; }
    }
}