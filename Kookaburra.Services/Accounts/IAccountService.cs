using Kookaburra.Domain.Model;
using System.Threading.Tasks;

namespace Kookaburra.Services.Accounts
{
    public interface IAccountService
    {
        Task<Account> GetAccountAsync(string operatorKey);

        Task<Operator> GetOperatorAsync(string operatorKey);       

        Task UpdateProfileAsync(string operatorKey, string firstName, string lastName, string email);        

        Task RecordOperatorActivityAsync(string operatorKey);

        Task ResetOperatorActivityAsync(string operatorKey);
    }
}