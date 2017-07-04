﻿namespace Kookaburra.Domain.Query.Transcript
{
    public class TranscriptQuery : IQuery<TranscriptQueryResult>
    {
        public TranscriptQuery(long conversationId, string operatorIdentity)
        {
            ConversationId = conversationId;
            OperatorIdentity = operatorIdentity;
        }

        public long ConversationId { get; }

        public string OperatorIdentity { get; }
    }
}