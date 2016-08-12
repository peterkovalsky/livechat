using System;

namespace Kookaburra.Domain.Model
{
    public class Message
    {
        public long Id { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public int? VisitorId { get; set; }

        public int? OperatorId { get; set; }


        public virtual Visitor Visitor { get; set; }

        public virtual Operator Operator { get; set; }
    }
}