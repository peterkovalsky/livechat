using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.ResumeOperator
{
    public class ResumeOperatorQuery : IQuery<Task<ResumeOperatorQueryResult>>
    {
        public ResumeOperatorQuery(string operatorIdentity)
        {
            OperatorIdentity = operatorIdentity;
        }

        public string OperatorIdentity { get; private set; }
    }
}