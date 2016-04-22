using Kookaburra.Domain.Model;

namespace Kookaburra.Domain.Repository
{
    public interface IOperatorRepository
    {
        Operator Get(string operatorIdentity);

        Account GetAccount(string operatorIdentity);
    }
}