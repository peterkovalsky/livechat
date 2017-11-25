using Kookaburra.Models.Account;
using Kookaburra.Services.Accounts;
using Kookaburra.ViewModels.Chat;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {                
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private ApplicationUserManager _userManager;

        private readonly IAccountService _accountService;


        public ChatController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<ActionResult> OperatorOnline()
        {
            var currentOperator = await _accountService.GetOperatorAsync(User.Identity.GetUserId());

            return PartialView("_OperatorOnline");
        }

        [HttpGet]
        [Route("chats/{id?}")]
        public async Task<ActionResult> ChatRoom(long? id)
        {
            var currentOperator = await _accountService.GetOperatorAsync(User.Identity.GetUserId());            
           
            var model = new RoomViewModel
            {
                CompanyId = currentOperator.Account.Key,
                OperatorName = currentOperator.FirstName,
                OperatorId = currentOperator.Id,
                ChatId = id,
                AccountStatus = await _accountService.CheckAccountAsync(User.Identity.GetUserId()),
                TrialExpiredViewModel = new TrialExpiredViewModel
                {
                    Name = currentOperator.FirstName,
                    TrialPeriodDays = currentOperator.Account.TrialPeriodDays
                }
            };

            return View(model);
        }
    }
}