using Kookaburra.Domain.Query;
using System.Threading.Tasks;

namespace Kookaburra.Domain.AvailableOperator
{
    public class AvailableOperatorQueryHandler : IQueryHandler<AvailableOperatorQuery, Task<AvailableOperatorQueryResult>>
    {      
        private readonly ChatSession _chatSession;

        public AvailableOperatorQueryHandler(ChatSession chatSession)
        {           
            _chatSession = chatSession;
        }

        public async Task<AvailableOperatorQueryResult> ExecuteAsync(AvailableOperatorQuery query)
        {
            var operatorSession = _chatSession.GetFirstAvailableOperator(query.AccountKey);

            if (operatorSession != null)
            {
                return new AvailableOperatorQueryResult
                {
                    OperatorId = operatorSession.Id,
                    OperatorName = operatorSession.Name,
                    OperatorConnectionIds = operatorSession.ConnectionIds
                };
            }

            return null;
        }
    }
}