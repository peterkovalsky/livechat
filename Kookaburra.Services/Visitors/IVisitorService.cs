using Kookaburra.Domain.Model;
using System.Threading.Tasks;

namespace Kookaburra.Services.Visitors
{
    public interface IVisitorService
    {
        Task<Visitor> GetVisitorAsync(string visitorKey);

        Task<Visitor> AddNewVisitorAsync(string accountKey, string visitorKey, string ip);

        Task UpdateVisitorGeolocationAsync(string visitorKey);
    }
}