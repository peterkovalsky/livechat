using System;

namespace Kookaburra.Services.Chats
{
    public class VisitorInfoResponse
    {
        public string SessionId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CurrentUrl { get; set; }

        public DateTime StartTime { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}