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
            var currentOperator = await _accountService.GetAsync(User.Identity.GetUserId());

            return PartialView("_OperatorOnline");
        }

        [HttpGet]
        [Route("chats")]
        public async Task<ActionResult> ChatRoom()
        {
            var currentOperator = await _accountService.GetAsync(User.Identity.GetUserId());

            var model = new RoomViewModel
            {
                CompanyId = currentOperator.Account.Identifier,
                OperatorName = currentOperator.FirstName,
                OperatorId = currentOperator.Id
            };

            return View(model);
        }
    }
}