using Kookaburra.Domain.Model;
using System;
using System.Collections.Generic;

namespace Kookaburra.Domain.Repository
{
    public interface IOperatorRepository
    {
        Operator Get(string operatorIdentity);

        IEnumerable<Operator> GetList(string accountKey);

        Account GetAccount(string operatorIdentity);

        void RecordOperatorActivity(string operatorIdentity);

        void ResetOperatorActivity(string operatorIdentity);

        DateTime? GetLastActivity(string operatorIdentity);
    }
}