using Kookaburra.Domain.Integration.Model;
using System.Threading.Tasks;

namespace Kookaburra.Domain.Integration
{
    public interface IGeoLocator
    {
        Task<VisitorLocation> GetLocationAsync(string ip);

        VisitorLocation GetLocation(string ip);
    }
}