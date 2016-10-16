using Kookaburra.Domain.Model;
using Kookaburra.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kookaburra.Services
{
    public class OperatorService
    {
        private readonly IOperatorRepository _operatorRepository;

        public OperatorService(IOperatorRepository operatorRepository)
        {
            _operatorRepository = operatorRepository;
        }


        public bool IsOnline(string operatorIdentifier)
        {
            var oper = _operatorRepository.Get(operatorIdentifier);

            return IsOnline(oper);
        }

        public bool IsOnline(Operator oper)
        {
            return (oper.LastActivity != null && DateTime.UtcNow < oper.LastActivity.Value.AddMinutes(30));
        }

        public List<Operator> GetOnlineOperators(string accountKey)
        {
            var operators = _operatorRepository.GetList(accountKey);

            return operators.Where(o => IsOnline(o)).ToList();
        }
    }
}