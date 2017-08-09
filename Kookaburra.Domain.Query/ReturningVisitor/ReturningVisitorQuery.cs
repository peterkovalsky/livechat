using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.ReturningVisitor
{
    public class ReturningVisitorQuery : IQuery<Task<ReturningVisitorQueryResult>>
    {
        public ReturningVisitorQuery(string accountKey, string visitorId)
        {
            VisitorId = visitorId;
            AccountKey = accountKey;
        }

        public string VisitorId { get; }

        public string AccountKey { get; }
    }
}