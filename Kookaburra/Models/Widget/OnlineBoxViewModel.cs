using System.ComponentModel.DataAnnotations;

namespace Kookaburra.ViewModels.Widget
{
    public class OnlineBoxViewModel
    {
        public string ClientKey { get; set; }

        [Required]
        public string VisitorName { get; set; }

        [EmailAddress]
        public string VisitorEmail { get; set; }
    }
}