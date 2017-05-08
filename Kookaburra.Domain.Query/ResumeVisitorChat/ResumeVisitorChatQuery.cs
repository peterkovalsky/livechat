using Kookaburra.Domain.Query;

namespace Kookaburra.Domain.ResumeVisitorChat
{
    public class ResumeVisitorChatQuery : IQuery<ResumeVisitorChatQueryResult>
    {
        public ResumeVisitorChatQuery(string visitorSessionId, string visitorConnectionId, string operatorIdentity)
        {
            VisitorSessionId = visitorSessionId;
            VisitorConnectionId = visitorConnectionId;
            OperatorIdentity = operatorIdentity;
        }

        public string VisitorSessionId { get; }

        public string VisitorConnectionId { get; }

        public string OperatorIdentity { get; }
    }
}