using System;
using System.Collections.Generic;
using System.Linq;

namespace Kookaburra.Domain
{
    public class ChatSession
    {
        private List<OperatorSession> Sessions { get; set; } = new List<OperatorSession>();


        public void AddOrUpdateOperator(int id, string identity, string name, string accountKey, string connectionId)
        {
            var operatorSession = GetOperatorByIdentity(identity);

            if (operatorSession == null)
            {
                var newOperator = new OperatorSession
                {
                    Id = id,
                    Identity = identity,
                    Name = name,
                    AccountKey = accountKey,
                    ConnectionIds = new List<string> { connectionId }
                };

                Sessions.Add(newOperator);
            }
            else
            {
                operatorSession.ConnectionIds.Add(connectionId);
            }
        }

        public void AddVisitor(int conversationId, int operatorId, string visitorConnectionId, int visitorId, string visitorName, string visitorSessionId)
        {
            var operatorSession = GetOperatorById(operatorId);
            if (operatorSession == null)
            {
                throw new ArgumentException("Session is missing for operator with id " + operatorId);
            }

            if (!operatorSession.Visitors.Any(v => v.SessionId == visitorSessionId))
            {
                var newVisitor = new VisitorSession
                {
                    Id = visitorId,
                    ConnectionIds = visitorConnectionId != null ? new List <string> { visitorConnectionId } : new List<string>(),
                    Name = visitorName,
                    ConversationId = conversationId,
                    SessionId = visitorSessionId
                };
                operatorSession.Visitors.Add(newVisitor);
            }
        }

        public void RemoveVisitor(string visitorSessionId)
        {
            var operatorSession = GetOperatorByVisitorSessionId(visitorSessionId);

            operatorSession.Visitors.RemoveAll(i => i.SessionId == visitorSessionId);
        }

        public void UpdateVisitor(string visitorSessionId, string newConnectionId)
        {
            var visitor = Sessions.SelectMany(s => s.Visitors).ToList().SingleOrDefault(v => v.SessionId == visitorSessionId);
            if (visitor != null)
            {
                visitor.ConnectionIds.Add(newConnectionId);
            }
        }

        public OperatorSession GetFirstAvailableOperator(string accountKey)
        {
            // get current active operators for an account
            var activeOperators = Sessions.Where(s => s.AccountKey == accountKey)
                .Select(s => new
                {
                    OperatorSession = s,
                    NumOfVisitors = s.Visitors.Count()
                })
                .ToList();

            if (activeOperators.Any())
            {
                // return the least loaded operator
                return activeOperators.OrderBy(o => o.NumOfVisitors).First().OperatorSession;
            }

            return null;
        }

        public OperatorSession GetOperatorByIdentity(string identity)
        {
            return Sessions.SingleOrDefault(s => s.Identity == identity);
        }

        public OperatorSession GetOperatorById(int id)
        {
            return Sessions.SingleOrDefault(s => s.Id == id);
        }

        public OperatorSession GetOperatorByOperatorConnId(string operatorConnectionId)
        {
            return Sessions.SingleOrDefault(s => s.ConnectionIds.Contains(operatorConnectionId));
        }

        public OperatorSession GetOperatorByVisitorConnId(string visitorConnectionId)
        {
            return Sessions.Where(s => s.Visitors.Any(v => v.ConnectionIds.Contains(visitorConnectionId))).SingleOrDefault();
        }

        public OperatorSession GetOperatorByVisitorSessionId(string visitorSessionId)
        {
            return Sessions.Where(s => s.Visitors.Any(v => v.SessionId == visitorSessionId)).SingleOrDefault();
        }

        public VisitorSession GetVisitorByVisitorConnId(string visitorConnectionId)
        {
            return Sessions.SelectMany(s => s.Visitors).ToList().SingleOrDefault(v => v.ConnectionIds.Contains(visitorConnectionId));
        }

        public VisitorSession GetVisitorByVisitorSessionId(string visitorSessionId)
        {
            return Sessions.SelectMany(s => s.Visitors).ToList().SingleOrDefault(v => v.SessionId == visitorSessionId);
        }
    }

    public class OperatorSession
    {
        public int Id { get; set; }

        public string Identity { get; set; }

        public string Name { get; set; }

        public string AccountKey { get; set; }

        public List<string> ConnectionIds { get; set; } = new List<string>();

        public List<VisitorSession> Visitors { get; set; } = new List<VisitorSession>();
    }

    public class VisitorSession
    {
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public string Name { get; set; }

        public List<string> ConnectionIds { get; set; } = new List<string>();

        public string SessionId { get; set; }
    }
}