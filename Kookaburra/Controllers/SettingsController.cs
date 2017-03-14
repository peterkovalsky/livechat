using Kookaburra.Domain.Common;
using Kookaburra.Domain.Repository;
using Kookaburra.Models.Settings;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class SettingsController : Controller
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

        public SettingsController(IAccountRepository accountRepository, IOperatorRepository operatorRepository)
        {
            _accountRepository = accountRepository;
            _operatorRepository = operatorRepository;
        }

        [HttpGet]
        [Route("settings/code")]
        public ActionResult Code()
        {
            var currentOperator = _operatorRepository.Get(User.Identity.GetUserId());
            string serverHost = Request.Url.Host + ":" + Request.Url.Port;

            var model = new CodeViewModel
            {
                CodeSnippet = new Code().GenerateCode(serverHost + "/widget/container", currentOperator.Account.Identifier)
            };
            
            return View(model);
        }
    }
}