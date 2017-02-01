namespace Kookaburra.Domain.Query.Result
{
    public class CurrentSessionQueryResult
    {
        public string VisitorName { get; set; }

        public string VisitorConnectionId { get; set; }

        public string VisitorSessionId { get; set; }

        public string OperatorName { get; set; }

        public string OperatorConnectionId { get; set; }

        public string OperatorSessionId { get; set; }
    }
}