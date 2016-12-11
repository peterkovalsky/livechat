using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;

namespace Kookaburra.Domain.Query.Handler
{
    public class VisitorSessionQueryHandler : IQueryHandler<VisitorSessionQuery, VisitorSessionQueryResult>
    {
        private readonly ChatSession _chatSession;

        public VisitorSessionQueryHandler(ChatSession chatSession)
        {
            _chatSession = chatSession;
        }


        public VisitorSessionQueryResult Execute(VisitorSessionQuery query)
        {
            var operatorSession = _chatSession.GetOperatorSession(query.VisitorConnectionId);

            return new VisitorSessionQueryResult {OperatorConnectionId = operatorSession .ConnectionId};
        }
    }
}