using System.Collections.Generic;

namespace Kookaburra.Models
{
    public class AlertViewModel
    {
        public List<string> Errors { get; set; }

        public string SuccessMessage { get; set; }

        public bool IsSuccess { get; set; }
    }
}