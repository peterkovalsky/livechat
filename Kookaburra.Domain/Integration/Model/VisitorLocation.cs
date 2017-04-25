namespace Kookaburra.Domain.Integration.Model
{
    public class VisitorLocation
    {
        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }
    }
}