using Kookaburra.Domain.Query;

namespace Kookaburra.Domain.AvailableOperator
{
    public class AvailableOperatorQueryHandler : IQueryHandler<AvailableOperatorQuery, AvailableOperatorQueryResult>
    {      
        private readonly ChatSession _chatSession;

        public AvailableOperatorQueryHandler(ChatSession chatSession)
        {           
            _chatSession = chatSession;
        }

        public AvailableOperatorQueryResult Execute(AvailableOperatorQuery query)
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