using AutoMapper;
using Kookaburra.Domain.Common;
using Kookaburra.Models.Home;
using Kookaburra.Services.Accounts;
using Kookaburra.Services.Chats;
using Kookaburra.Services.OfflineMessages;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
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
        private readonly IChatService _chatService;

        public HomeController(IAccountService accountService, IOfflineMessageService offlineMessageService, IChatService chatService)
        {
            _accountService = accountService;
            _offlineMessageService = offlineMessageService;
            _chatService = chatService;
        }


        [HttpGet]
        [Route("")]
        [Route("dashboard")]
        public ActionResult Dashboard()
        {
            return View();
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
            var currentOperator = await _accountService.GetOperatorAsync(User.Identity.GetUserId());
            string serverHost = Request.Url.Host + ":" + Request.Url.Port;

            var model = new PreviewViewModel
            {
                Code = new Code().GenerateCode(serverHost + "/widget/container", currentOperator.Account.Identifier)
            };

            return View(model);
        }
    }
}