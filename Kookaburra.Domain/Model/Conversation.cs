using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Domain.Model
{
    public class Conversation
    {
        [Key]
        public long Id { get; set; }

        public int VisitorId { get; set; }

        public virtual Visitor Visitor { get; set; }

        public int OperatorId { get; set; }

        public Operator Operator { get; set; }

        public DateTime TimeStarted { get; set; }

        public DateTime? TimeFinished { get; set; }

        public string Page { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}