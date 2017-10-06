using Kookaburra.Domain.Common;
using Kookaburra.Models.Home;
using Kookaburra.Services.Accounts;
using Kookaburra.Services.OfflineMessages;
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
        private readonly IOfflineMessageService _offlineMessageService;


        public HomeController(IAccountService accountService, IOfflineMessageService offlineMessageService)
        {
            _accountService = accountService;
            _offlineMessageService = offlineMessageService;
        }


        [HttpGet]
        [Route("")]
        [Route("dashboard")]
        public async Task<ActionResult> Dashboard()
        {
            var viewModel = new DashboardViewModel
            {
                NewOfflineMessages = await _offlineMessageService.TotalNewOfflineMessagesAsync(User.Identity.GetUserId())
            };

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Route("preview")]
        public async Task<ActionResult> Preview()
        {
            var currentOperator = await _accountService.GetAsync(User.Identity.GetUserId());
            string serverHost = Request.Url.Host + ":" + Request.Url.Port;

            var model = new PreviewViewModel
            {
                Code = new Code().GenerateCode(serverHost + "/widget/container", currentOperator.Account.Identifier)
            };

            return View(model);
        }
    }
}