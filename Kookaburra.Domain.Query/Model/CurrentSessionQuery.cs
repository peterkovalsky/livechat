using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Model
{
    public class CurrentSessionQuery : IQuery<CurrentSessionQueryResult>
    {
        public string VisitorConnectionId { get; set; }

        public string VisitorSessionId { get; set; }
    }
}