using Kookaburra.Domain.Model;
using System;

namespace Kookaburra.Domain.Repository
{
    public interface IAccountRepository
    {
        Account Get(string identifier);   
    }
}