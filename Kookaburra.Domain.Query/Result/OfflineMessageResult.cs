using System;

namespace Kookaburra.Domain.Query.Result
{
    public class OfflineMessageResult
    {
        public string VisitorName { get; set; }

        public string Message { get; set; }

        public DateTime TimeSent { get; set; }

        public string Country { get; set; }

        public string City { get; set; }       
    }
}