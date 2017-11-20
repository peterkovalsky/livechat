using Kookaburra.Domain;
using System;
using System.Collections.Generic;

namespace Kookaburra.Services.Chats
{
    public class TranscriptResponse
    {
        public DateTime TimeStarted { get; set; }

        public Duration Duration { get; set; }

        public VisitorResponse Visitor { get; set; }

        public List<MessageResponse> Messages { get; set; }
    }

    public class VisitorResponse
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string City { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}