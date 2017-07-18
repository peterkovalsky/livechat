﻿using Kookaburra.Domain.Query;
using System.Threading.Tasks;

namespace Kookaburra.Domain.ResumeVisitorChat
{
    public class ResumeVisitorChatQuery : IQuery<Task<ResumeVisitorChatQueryResult>>
    {
        public ResumeVisitorChatQuery(string visitorSessionId, string visitorConnectionId)
        {
            VisitorSessionId = visitorSessionId;
            VisitorConnectionId = visitorConnectionId;        
        }

        public string VisitorSessionId { get; }

        public string VisitorConnectionId { get; }

        public string OperatorIdentity { get; }
    }
}