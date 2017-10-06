using Kookaburra.Domain.Model;
using System.Threading.Tasks;

namespace Kookaburra.Services.Accounts
{
    public interface IAccountService
    {
        Task<Operator> GetProfileAsync(string operatorKey);

        Task UpdateProfileAsync(string operatorKey, string firstName, string lastName, string email);

        Task<Operator> GetAsync(string identity);

        Task RecordOperatorActivityAsync(string operatorIdentity);

        Task ResetOperatorActivityAsync(string operatorIdentity);
    }
}