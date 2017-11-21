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


        public async Task<Visitor> GetVisitorAsync(string visitorKey)
        {
            var visitor = await _context.Visitors.SingleOrDefaultAsync(v => v.Identifier == visitorKey);
            if (visitor == null)
            {
                throw new ArgumentException($"Visitor {visitorKey} doesn't exist.");
            }

            return visitor;
        }

        public async Task<Visitor> AddNewVisitorAsync(string accountKey, string visitorKey, string ip)
        {
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.Key == accountKey);
            if (account == null)
            {
                throw new ArgumentException($"Account {accountKey} doesn't exist.");
            }

            var visitor = new Visitor
            {
                Identifier = visitorKey,
                IpAddress = ip,
                Account = account
            };       

            _context.Visitors.Add(visitor);

            await _context.SaveChangesAsync();

            return visitor;
        }

        public async Task UpdateVisitorGeolocationAsync(string visitorKey)
        {
            var visitor = await GetVisitorAsync(visitorKey);

            if (!string.IsNullOrWhiteSpace(visitor.IpAddress))
            {
                var location = await _geoLocator.GetLocationAsync(visitor.IpAddress);
                if (location != null)
                {
                    visitor.Country = location.Country;
                    visitor.CountryCode = location.CountryCode;
                    visitor.Region = location.Region;
                    visitor.City = location.City;
                    visitor.Latitude = location.Latitude;
                    visitor.Longitude = location.Longitude;

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}