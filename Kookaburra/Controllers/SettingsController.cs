using Kookaburra.Common;
using Kookaburra.Domain.Common;
using Kookaburra.Models.Settings;
using Kookaburra.Services.Accounts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class SettingsController : Controller
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

      
        public SettingsController(IAccountService accountService)
        {
            _accountService = accountService;
        }


        [HttpGet]
        [Route("installation")]
        public async Task<ActionResult> Code()
        {
            var currentOperator = await _accountService.GetOperatorAsync(User.Identity.GetUserId());            

            var model = new CodeViewModel
            {
                CodeSnippet = new Code().GenerateCode(WebHelper.Host, currentOperator.Account.Key)
            };
            
            return View(model);
        }
    }
}