using System.Threading.Tasks;

namespace Kookaburra.Domain.Query.Transcript
{
    public class TranscriptQuery : IQuery<Task<TranscriptQueryResult>>
    {
        public TranscriptQuery(long conversationId, string accountKey)
        {
            ConversationId = conversationId;
            AccountKey = accountKey;
        }

        public long ConversationId { get; }      

        public string AccountKey { get; }
    }
}