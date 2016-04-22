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
    }
}