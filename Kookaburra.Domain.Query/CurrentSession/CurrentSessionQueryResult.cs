using System.Collections.Generic;

namespace Kookaburra.Domain.Query.CurrentSession
{
    public class CurrentSessionQueryResult
    {
        public string VisitorName { get; set; }

        public List<string> VisitorConnectionIds { get; set; }

        public string VisitorSessionId { get; set; }

        public string OperatorName { get; set; }

        public List<string> OperatorConnectionIds { get; set; }      
    }
}