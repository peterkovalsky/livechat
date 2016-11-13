using System.ComponentModel.DataAnnotations;

namespace Kookaburra.ViewModels.Widget
{
    public class OfflineBoxViewModel
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

        public bool ThankYou { get; set; }
    }
}