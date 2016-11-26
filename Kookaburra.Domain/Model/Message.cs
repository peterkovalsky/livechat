using System;
using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Domain.Model
{
    public class Message
    {
        [Key]
        public long Id { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public string SentBy { get; set; }

        public int ConversationId { get; set; }

        public virtual Conversation Conversation { get; set; }
    }
} 