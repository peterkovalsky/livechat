using System;
using System.Collections.Generic;
using System.Linq;

namespace Kookaburra.Services
{
    public class ChatSession
    {
        public ChatSession()
        {
            Sessions = new List<OperatorSession>();
        }

        private List<OperatorSession> Sessions { get; set; }

        public void AddOperator(int id, string name, string accountKey, string connectionId)
        {
            if (!Sessions.Any(s => s.Id == id))
            {
                var newOperator = new OperatorSession
                {
                    Id = id,
                    Name = name,
                    AccountKey = accountKey,
                    ConnectionId = connectionId
                };

                Sessions.Add(newOperator);
            }
        }
         
        public void AddVisitor(string operatorConnectionId, string visitorConnectionId, long visitorId, string visitorName)
        {
            if (!Sessions.Any(s => s.ConnectionId == operatorConnectionId && s.Visitors.Any(v => v.ConnectionId == visitorConnectionId)))
            {
                var operatorSession = Sessions.Where(s => s.ConnectionId == operatorConnectionId).SingleOrDefault();
                if (operatorSession == null)
                {
                    throw new ArgumentException("Operator session is missing for connection id " + operatorConnectionId);
                }

                var newVisitor = new VisitorSession
                {
                    Id = visitorId,
                    ConnectionId = visitorConnectionId,
                    Name = visitorName
                };
                operatorSession.Visitors.Add(newVisitor);
            }
        }

        public string GetFirstAvailableOperator(string accountKey)
        {
            // get current active operators for an account
            var activeOperators = Sessions.Where(s => s.AccountKey == accountKey)
                .Select(s => new {
                    OperatorConnectionId = s.ConnectionId,
                    NumOfVisitors = s.Visitors.Count()
                })
                .ToList();

            if (activeOperators.Any())
            {
                // return less loaded operator
                return activeOperators.OrderBy(o => o.NumOfVisitors).First().OperatorConnectionId;
            }     

            return null;
        }
    }  

    public class OperatorSession
    {
        public OperatorSession()
        {
            Visitors = new List<VisitorSession>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string AccountKey { get; set; }

        public string ConnectionId { get; set; }
    
        public List<VisitorSession> Visitors { get; set; }
    }

    public class VisitorSession
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string ConnectionId { get; set; }
    }
}