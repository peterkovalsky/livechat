using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.CurrentSession
{
    public class CurrentSessionQuery : IQuery<Task<CurrentSessionQueryResult>>
    {
        public CurrentSessionQuery()
        {                       
        }

        public string VisitorConnectionId { get; set; }

        public string VisitorSessionId { get; set; }    

        public string AccountKey { get; }
    }
}