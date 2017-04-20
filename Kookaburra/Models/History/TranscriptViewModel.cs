using Kookaburra.Domain;
using System;
using System.Collections.Generic;

namespace Kookaburra.Models.History
{
    public class TranscriptViewModel
    {
        public DateTime TimeStarted { get; set; }        

        public Duration Duration { get; set; }

        public VisitorViewModel Visitor { get; set; }

        public List<MessageViewModel> Messages { get; set; }
    }   
}