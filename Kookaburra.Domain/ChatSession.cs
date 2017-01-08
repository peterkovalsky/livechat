﻿using Kookaburra.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kookaburra.Domain
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

        public void AddVisitor(int conversationId, string operatorConnectionId, string visitorConnectionId, int visitorId, string visitorName, string visitorSessionId)
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
                    Name = visitorName,
                    ConversationId = conversationId,
                    SessionId = visitorSessionId
                };
                operatorSession.Visitors.Add(newVisitor);
            }
        }

        public void RemoveVisitor(string visitorConnectionId)
        {
            Sessions.SelectMany(s => s.Visitors).ToList().RemoveAll(i => i.ConnectionId == visitorConnectionId);
        }

        public void RemoveOperator(string operatorConnectionId)
        {
            Sessions.RemoveAll(i => i.ConnectionId == operatorConnectionId);
        }

        public void UpdateVisitor(string visitorSessionId, string newConnectionId)
        {
            var visitor = Sessions.SelectMany(s => s.Visitors).ToList().SingleOrDefault(v => v.SessionId == visitorSessionId);
            if (visitor != null)
            {
                visitor.ConnectionId = newConnectionId;
            }
        }

        public bool AnyOperatorAvailable(string accountKey)
        {
            // get current active operators for an account
            var activeOperators = Sessions.Where(s => s.AccountKey == accountKey)
                .Select(s => new {
                    OperatorConnectionId = s.ConnectionId,
                    NumOfVisitors = s.Visitors.Count()
                })
                .ToList();

            return activeOperators.Any();
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

        public OperatorSession GetOperator(int id)
        {
            return Sessions.Where(s => s.Id == id).SingleOrDefault();
        }

        public int GetOperatorId(string operatorConnectionId)
        {
            var operatorObj = Sessions.SingleOrDefault(s => s.ConnectionId == operatorConnectionId);
            if (operatorObj == null)
            {
                throw new OperatorDisconnectedException("Operator has been disconnected. ID: " + operatorConnectionId);
            }

            return operatorObj.Id;
        }

        public int GetVisitorId(string visitorConnectionId)
        {
            var visitor = Sessions.SelectMany(s => s.Visitors).ToList().SingleOrDefault(v => v.ConnectionId == visitorConnectionId);
            if (visitor == null)
            {
                throw new VisitorDisconnectedException("Visitor has been disconnected. ID: " + visitorConnectionId);
            }

            return visitor.Id;
        }

        public int GetConversationId(string visitorConnectionId)
        {
            var visitor = Sessions.SelectMany(s => s.Visitors).ToList().SingleOrDefault(v => v.ConnectionId == visitorConnectionId);
            if (visitor == null)
            {
                throw new VisitorDisconnectedException("Visitor has been disconnected. ID: " + visitorConnectionId);
            }

            return visitor.ConversationId;
        }

        public bool IsConversationAlive(string visitorSessionId)
        {
            var visitor = Sessions.SelectMany(s => s.Visitors).ToList().SingleOrDefault(v => v.SessionId == visitorSessionId);

            return visitor != null;
        }

        public OperatorSession GetOperatorSession(string visitorConnectionId)
        {
            var operatorSession = Sessions.Where(s => s.Visitors.Any(v => v.ConnectionId == visitorConnectionId)).SingleOrDefault();
            if (operatorSession == null)
            {
                throw new ArgumentException("Visitor doen't have an operator");
            }

            return operatorSession;
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
        public int Id { get; set; }

        public int ConversationId { get; set; }

        public string Name { get; set; }

        public string ConnectionId { get; set; }

        public string SessionId { get; set; }
    }
}