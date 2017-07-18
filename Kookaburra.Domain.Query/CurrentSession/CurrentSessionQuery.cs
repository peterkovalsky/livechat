using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.CurrentSession
{
    public class CurrentSessionQuery : IQuery<Task<CurrentSessionQueryResult>>
    {
        public CurrentSessionQuery(string operatorIdentity)
        {
            OperatorIdentity = operatorIdentity;
        }

        public string VisitorConnectionId { get; set; }

        public string VisitorSessionId { get; set; }

        public string OperatorIdentity { get; }
    }
}