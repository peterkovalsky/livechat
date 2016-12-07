using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Repository;

namespace Kookaburra.Domain.Query.Handler
{
    public class AvailableOperatorQueryHandler : IQueryHandler<AvailableOperatorQuery, AvailableOperatorQueryResult>
    {
        private readonly KookaburraContext _context;
        private readonly ChatSession _chatSession;

        public AvailableOperatorQueryHandler(KookaburraContext context, ChatSession chatSession)
        {
            _context = context;
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