using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Domain.Model
{
    public class Visitor
    {
        public long Id { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(50)]
        public string IpAddress { get; set; }

        [StringLength(200)]
        public string Country { get; set; }

        [StringLength(10)]
        public string CountryCode { get; set; }

        [StringLength(1000)]
        public string Region { get; set; }

        [StringLength(1000)]
        public string City { get; set; }

        public decimal Latitude { get; set; }
    
        public decimal Longitude { get; set; }

        [StringLength(100)]
        public string Identifier { get; set; }      

        public virtual IEnumerable<Conversation> Conversations { get; set; }

        public virtual IEnumerable<OfflineMessage> OfflineMessages { get; set; }

        public virtual Account Account { get; set; }

        public int AccountId { get; set; }
    }
}