using Kookaburra.Domain.Common;
using Kookaburra.Domain.Repository;
using Kookaburra.Models.Home;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;

        private readonly IAccountRepository _accountRepository;

        private readonly IOperatorRepository _operatorRepository;

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


        public HomeController(IAccountRepository accountRepository, IOperatorRepository operatorRepository)
        {
            _accountRepository = accountRepository;
            _operatorRepository = operatorRepository;           
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
        public ActionResult Preview()
        {
            var currentOperator = _operatorRepository.Get(User.Identity.GetUserId());
            string serverHost = Request.Url.Host + ":" + Request.Url.Port;

            var model = new PreviewViewModel
            {
                Code = new Code().GenerateCode(serverHost + "/widget/container", currentOperator.Account.Identifier)
            };

            return View(model);
        }
    }
}