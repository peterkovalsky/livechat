using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Models.Widget
{
    public class OfflineViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public string AccountKey { get; set; }
    }
}