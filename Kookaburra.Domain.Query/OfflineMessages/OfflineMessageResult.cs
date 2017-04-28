using System;

namespace Kookaburra.Domain.Query.OfflineMessages
{
    public class OfflineMessageResult
    {
        public long Id { get; set; }

        public string VisitorName { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public bool IsRead { get; set; }

        public DateTime TimeSent { get; set; }

        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string City { get; set; }       
    }
}