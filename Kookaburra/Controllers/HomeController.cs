using Kookaburra.Common;
using Kookaburra.Domain.Common;
using Kookaburra.Models.Home;
using Kookaburra.Services.Accounts;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class HomeController : Controller
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

        public HomeController(IAccountService accountService)
        {
            _accountService = accountService;             
        }


        [HttpGet]
        [Route("")]
        [Route("dashboard")]
        public ActionResult Dashboard()
        {
            return View();
        }   

        [Route("preview")]
        public async Task<ActionResult> Preview()
        {
            var currentOperator = await _accountService.GetOperatorAsync(User.Identity.GetUserId());            

            var model = new PreviewViewModel
            {
                Code = new Code().GenerateCode(WebHelper.Host, currentOperator.Account.Key)
            };

            return View(model);
        }
    }
}