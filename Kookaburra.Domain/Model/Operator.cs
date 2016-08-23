using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Domain.Model
{
    public class Operator
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string Identity { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Type { get; set; }


        public int AccountId { get; set; }

        public DateTime? LastActivity { get; set; }

        public virtual Account Account { get; set; }

        public virtual IEnumerable<Message> Messages { get; set; }
    }
} 