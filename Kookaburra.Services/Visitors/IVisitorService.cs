using Kookaburra.Domain.Model;
using System.Threading.Tasks;

namespace Kookaburra.Services.Visitors
{
    public interface IVisitorService
    {
        Task AddNewVisitorAsync(string accountKey, string visitorId, string ip);
    }
}