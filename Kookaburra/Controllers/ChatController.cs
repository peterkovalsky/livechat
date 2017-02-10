using Kookaburra.Domain.Repository;
using Kookaburra.ViewModels.Chat;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private ApplicationUserManager _userManager;

        private readonly IOperatorRepository _operatorRepositor;

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


        public ChatController(IOperatorRepository operatorRepositor)
        {
            _operatorRepositor = operatorRepositor;
        }

        public ActionResult OperatorOnline()
        {
            var currentOperator = _operatorRepositor.Get(User.Identity.GetUserId());

            return PartialView("_OperatorOnline");
        }

        [HttpGet]
        [Route("chat-room")]
        public ActionResult ChatRoom()
        {
            var currentOperator = _operatorRepositor.Get(User.Identity.GetUserId());

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