using System;
using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Domain.Model
{
    public class OfflineMessage
    {
        [Key]
        public long Id { get; set; }

        public string Message { get; set; }

        public DateTime DateSent { get; set; }

        public int? VisitorId { get; set; }

        public bool IsRead { get; set; }

        public virtual Visitor Visitor { get; set; }

        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}