using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class VisitorSessionQuery : IQuery<VisitorSessionQueryResult>
    {
        public VisitorSessionQuery(string visitorConnectionId)
        {
            VisitorConnectionId = visitorConnectionId;
        }

        public string VisitorConnectionId { get; private set; }
    }
}