namespace Kookaburra.Domain.Query.ResumeOperator
{
    public class ResumeOperatorQuery : IQuery<ResumeOperatorQueryResult>
    {
        public ResumeOperatorQuery(string operatorIdentity)
        {
            OperatorIdentity = operatorIdentity;
        }

        public string OperatorIdentity { get; private set; }
    }
}