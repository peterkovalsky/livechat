using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class CurrentSessionQuery : IQuery<CurrentSessionQueryResult>
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