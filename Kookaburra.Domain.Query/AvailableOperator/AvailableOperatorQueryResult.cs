using System.Collections.Generic;

namespace Kookaburra.Domain.AvailableOperator
{
    public class AvailableOperatorQueryResult
    {
        public int OperatorId { get; set; }

        public string OperatorName { get; set; }

        public List<string> OperatorConnectionIds { get; set; }
    }
}