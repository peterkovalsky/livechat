using System;

namespace Kookaburra.Domain.Query.Result
{
    public class VisitorInfoResult
    {
        public string SessionId { get; set; }

        public string Name { get; set; }

        public string CurrentUrl { get; set; }

        public DateTime StartTime { get; set; }

        public string Location { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}