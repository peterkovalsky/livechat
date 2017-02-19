using System.Collections.Generic;

namespace Kookaburra.Domain.Query.Result
{
    public class AvailableOperatorQueryResult
    {
        public int OperatorId { get; set; }

        public List<string> OperatorConnectionIds { get; set; }
    }
}