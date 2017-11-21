using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Domain.Model
{
    public class Account
    {
        public int Id { get; set; }

        [StringLength(500)]
        public string Website { get; set; }

        public bool IsTrial { get; set; }

        public DateTime SignUpDate { get; set; }

        public DateTime TrialExpiryDate { get; set; }

        [StringLength(50)]
        public string Identifier { get; set; }

        public virtual ICollection<Operator> Operators { get; set; }

        public virtual ICollection<Visitor> Visitors { get; set; }
    }
} 