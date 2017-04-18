using System;
using System.Collections.Generic;

namespace Kookaburra.Models.History
{
    public class TranscriptViewModel
    {
        public DateTime TimeStarted { get; set; }

        public DateTime TimeFinished { get; set; }

        public int Duration
        {
            get
            {
                return (TimeFinished - TimeStarted).Minutes;
            }
        }

        public VisitorViewModel Visitor { get; set; }

        public List<MessageViewModel> Messages { get; set; }
    }
}