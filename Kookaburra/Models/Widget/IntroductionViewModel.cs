using System.ComponentModel.DataAnnotations;

namespace Kookaburra.Models.Widget
{
    public class IntroductionViewModel
    {
        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string AccountKey { get; set; }
    }
}