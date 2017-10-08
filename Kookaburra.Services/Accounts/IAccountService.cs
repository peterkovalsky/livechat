using Kookaburra.Domain.Model;
using System.Threading.Tasks;

namespace Kookaburra.Services.Accounts
{
    public interface IAccountService
    {
        Task<Account> GetAccountAsync(string operatorIdentity);

        Task<Operator> GetOperatorAsync(string operatorIdentity);       

        Task UpdateProfileAsync(string operatorIdentity, string firstName, string lastName, string email);        

        Task RecordOperatorActivityAsync(string operatorIdentity);

        Task ResetOperatorActivityAsync(string operatorIdentity);
    }
}