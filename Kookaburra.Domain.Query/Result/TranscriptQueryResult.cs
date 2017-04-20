using System;
using System.Collections.Generic;

namespace Kookaburra.Domain.Query.Result
{
    public class TranscriptQueryResult
    {
        public DateTime TimeStarted { get; set; }

        public Duration Duration { get; set; }       

        public VisitorResult Visitor { get; set; }

        public List<MessageResult> Messages { get; set; }
    }

    public class VisitorResult
    {
        public string Name { get; set; }
     
        public string Email { get; set; }
      
        public string Country { get; set; }

        public string City { get; set; }
       
        public decimal Latitude { get; set; }
      
        public decimal Longitude { get; set; }
    }
}