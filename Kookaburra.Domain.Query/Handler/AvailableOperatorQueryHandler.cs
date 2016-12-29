using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Handler
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
            var operatorConnectionId = _chatSession.GetFirstAvailableOperator(query.AccountKey);

            if (!string.IsNullOrWhiteSpace(operatorConnectionId))
            {
                return new AvailableOperatorQueryResult
                {
                    OperatorId = _chatSession.GetOperatorId(operatorConnectionId),
                    OperatorConnectionId = operatorConnectionId
                };
            }

            return null;
        }
    }
}