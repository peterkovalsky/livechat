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
                var operatorSession = _chatSession.GetOperatorByOperatorConnId(operatorConnectionId);

                return new AvailableOperatorQueryResult
                {
                    OperatorId = operatorSession.Id,
                    OperatorConnectionId = operatorConnectionId
                };
            }

            return null;
        }
    }
}