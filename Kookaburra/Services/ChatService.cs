using Kookaburra.Domain.Repository;
using Microsoft.AspNet.Identity.Owin;
using System.Web;

namespace Kookaburra.Services
{
    public class ChatService
    {
        private ApplicationUserManager _userManager;

        private readonly IAccountRepository _accountRepository;

        private readonly IOperatorRepository _operatorRepository;


        public ApplicationUserManager UserManager
        {
            get
            {                
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ChatService(IAccountRepository accountRepository, IOperatorRepository operatorRepository)
        {
            _accountRepository = accountRepository;
            _operatorRepository = operatorRepository;
        }       
    }
}