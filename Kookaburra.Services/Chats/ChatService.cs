using Kookaburra.Domain;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Model;
using Kookaburra.Repository;
using Kookaburra.Services.Accounts;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Kookaburra.Services.Chats
{
    public class ChatService : IChatService
    {
        private readonly KookaburraContext _context;
        private readonly IAccountService _accountService;
       

        public ChatService(KookaburraContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;           
        }
        

  

                    

       
    }
}