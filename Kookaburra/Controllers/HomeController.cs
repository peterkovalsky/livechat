using Kookaburra.Domain.Common;
using Kookaburra.Domain.Repository;
using Kookaburra.Models.Home;
using Kookaburra.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationUserManager _userManager;

        public IAccountRepository AccountRepository { get; set; }

        public IOperatorRepository OperatorRepository { get; set; }

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

        public HomeController()
        {
            AccountRepository = new AccountRepository(new KookaburraContext("name=DefaultConnection"));
            OperatorRepository = new OperatorRepository(new KookaburraContext("name=DefaultConnection"));
        }


        public ActionResult Index()
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
            var currentOperator = OperatorRepository.Get(User.Identity.GetUserId());

            var model = new PreviewViewModel
            {
                Code = new Code().GenerateCode("local.kookaburra.com/chatboxstyle", currentOperator.Account.Identifier)
            };

            return View(model);
        }
    }
}