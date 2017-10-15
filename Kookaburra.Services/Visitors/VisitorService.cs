using Kookaburra.Domain.Integration;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Kookaburra.Services.Visitors
{
    public class VisitorService : IVisitorService
    {
        private readonly KookaburraContext _context;

        private readonly IGeoLocator _geoLocator;

        public VisitorService(KookaburraContext context, IGeoLocator geoLocator)
        {
            _context = context;
            _geoLocator = geoLocator;
        }


        public async Task AddNewVisitorAsync(string accountKey, string visitorId, string ip)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Identifier == accountKey);
            if (account == null)
            {
                throw new ArgumentException($"Account {accountKey} doesn't exist.");
            }

            var visitor = new Visitor
            {
                SessionId = visitorId,
                IpAddress = ip,
                Account = account
            };

            try
            {
                var location = await _geoLocator.GetLocationAsync(ip);

                if (location != null)
                {
                    visitor.Country = location.Country;
                    visitor.CountryCode = location.CountryCode;
                    visitor.Region = location.Region;
                    visitor.City = location.City;
                    visitor.Latitude = location.Latitude;
                    visitor.Longitude = location.Longitude;
                }
            }
            catch (Exception ex)
            {
                // log error
            }
           
            _context.Visitors.Add(visitor);

            await _context.SaveChangesAsync();
        }

        public void UpdateVisitorGeoLocation(long visiotrId)
        {

        }
    }
}