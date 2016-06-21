using System.Collections.Generic;
using System.Linq;

namespace Kookaburra.Services
{
    public static class ChatOperation
    {
        public static List<ChatSession> CurrentState = new List<ChatSession>();

        private static Dictionary<string, string> Clients = new Dictionary<string, string>();
        private static Dictionary<string, string> Operators = new Dictionary<string, string>();


        public static void ConnectOperator(string accountKey, int operatorId, string operatorConnectionId, string operatorName)
        {
            Operators.Add(operatorConnectionId, operatorName);
            CurrentState.Add(new ChatSession { AccountKey = accountKey, OperatorId = operatorId, OperatorConnectionId = operatorConnectionId });
        }

        public static void DisconnectOperator(string operatorConnectionId)
        {
            Operators.Remove(operatorConnectionId);
            CurrentState.RemoveAll(s => s.OperatorConnectionId == operatorConnectionId);
        }

        public static void ConnectVisitor(string visitorConnectionId, string operatorConnectionId, string visitorName)
        {
            Clients.Add(visitorConnectionId, visitorName);
            var connectedOperator = CurrentState.Where(s => s.OperatorConnectionId == operatorConnectionId).SingleOrDefault();
            if (connectedOperator != null)
            {
                if (!connectedOperator.Visitos.Any(c => c == visitorConnectionId))
                    connectedOperator.Visitos.Add(visitorConnectionId);
            }
        }

        public static void DisconnectVisitor(string visitorId)
        {
            Clients.Remove(visitorId);
            var currentOperator = CurrentState.Where(s => s.Visitos.Any(c => c == visitorId)).SingleOrDefault();
            if (currentOperator != null)
            {
                currentOperator.Visitos.Remove(visitorId);
            }
        }

        public static void Disconnect(string connectionId)
        {
            DisconnectVisitor(connectionId);
            DisconnectOperator(connectionId);
        }

        public static string GetFirstAvailableOperator(string accountKey)
        {
            var operators = CurrentState.Where(s => s.AccountKey == accountKey).ToList();
            if (operators.Any())
            {
                var operatorsWithClients = operators.Select(o => new { OperatorConnectionId = o.OperatorConnectionId, NumOfClients = o.Visitos.Count() });
                var lessLoadedOperator = operatorsWithClients.OrderBy(llo => llo.NumOfClients).FirstOrDefault();

                return lessLoadedOperator.OperatorConnectionId;
            }

            return null;
        }

        public static string GetOperatorConnectionId(string visitorConnectionId)
        {
            return CurrentState.Where(s => s.Visitos.Any(c => c == visitorConnectionId)).Select(c => c.OperatorConnectionId).SingleOrDefault();
        }

        public static string GetOperatorName(string operatorId)
        {
            string name = null;
            Operators.TryGetValue(operatorId, out name);

            return name;
        }

        public static string GetClientName(string clientId)
        {
            string name = null;
            Clients.TryGetValue(clientId, out name);

            return name;
        }
    }

    public class ChatSession
    {
        public ChatSession()
        {
            Visitos = new List<string>();
        }

        public string AccountKey { get; set; }

        public int OperatorId { get; set; }

        public string OperatorConnectionId { get; set; }

        public List<string> Visitos { get; set; }
    }
}