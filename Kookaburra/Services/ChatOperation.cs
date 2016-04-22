using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services
{
    public static class ChatOperation
    {
        public static List<ChatSession> CurrentState = new List<ChatSession>();

        private static Dictionary<string, string> Clients = new Dictionary<string, string>();
        private static Dictionary<string, string> Operators = new Dictionary<string, string>();


        public static void ConnectOperator(string companyId, string operatorId, string name)
        {
            Operators.Add(operatorId, name);
            CurrentState.Add(new ChatSession { CompanyId = companyId, OperatorId = operatorId });
        }

        public static void DisconnectOperator(string operatorId)
        {
            Operators.Remove(operatorId);
            CurrentState.RemoveAll(s => s.OperatorId == operatorId);
        }

        public static void ConnectClient(string clientId, string operatorId, string clientName)
        {
            Clients.Add(clientId, clientName);
            var connectedOperator = CurrentState.Where(s => s.OperatorId == operatorId).SingleOrDefault();
            if (connectedOperator != null)
            {
                if (!connectedOperator.Clients.Any(c => c == clientId))
                    connectedOperator.Clients.Add(clientId);
            }
        }

        public static void DisconnectClient(string clientId)
        {
            Clients.Remove(clientId);
            var currentOperator = CurrentState.Where(s => s.Clients.Any(c => c == clientId)).SingleOrDefault();
            if (currentOperator != null)
            {
                currentOperator.Clients.Remove(clientId);
            }
        }

        public static void Disconnect(string connectionId)
        {
            DisconnectClient(connectionId);
            DisconnectOperator(connectionId);
        }

        public static string GetFirstAvailableOperator(string companyId)
        {
            var operators = CurrentState.Where(s => s.CompanyId == companyId).ToList();
            if (operators.Any())
            {
                var operatorsWithClients = operators.Select(o => new { OperatorId = o.OperatorId, NumOfClients = o.Clients.Count() });
                var lessLoadedOperator = operatorsWithClients.OrderBy(llo => llo.NumOfClients).FirstOrDefault();

                return lessLoadedOperator.OperatorId;
            }

            return null;
        }

        public static string GetOperatorId(string clientId)
        {
            return CurrentState.Where(s => s.Clients.Any(c => c == clientId)).Select(c => c.OperatorId).SingleOrDefault();
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
            Clients = new List<string>();
        }

        public string CompanyId { get; set; }

        public string OperatorId { get; set; }

        public List<string> Clients { get; set; }
    }
}
