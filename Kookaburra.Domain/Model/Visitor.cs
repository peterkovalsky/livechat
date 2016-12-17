﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Domain.Model
{
    public class Visitor
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(200)]
        public string Location { get; set; }

        [StringLength(1000)]
        public string SessionId { get; set; }      

        public virtual IEnumerable<Conversation> Conversations { get; set; }

        public virtual IEnumerable<OfflineMessage> OfflineMessages { get; set; }
    }
}