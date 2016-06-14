using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Kookaburra.Domain.Repository;
using Kookaburra.Repository;
using Kookaburra.ViewModels.Chat;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class ChatController : Controller
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


        public ChatController()
        {
            AccountRepository = new AccountRepository(new KookaburraContext("name=DefaultConnection"));
            OperatorRepository = new OperatorRepository(new KookaburraContext("name=DefaultConnection"));
        }


        [HttpGet]
        [Route("chat-room")]
        [Route("")]
        public ActionResult Room()
        {            
            var currentOperator = OperatorRepository.Get(User.Identity.GetUserId());

            var model = new RoomViewModel
            {
                CompanyId = currentOperator.Account.Identifier,
                OperatorName = currentOperator.FirstName,
                OperatorId = currentOperator.Id
            };

            return View(model);
        }

        [HttpGet]
        [Route("chat-room-old")]
        public ActionResult RoomOld()
        {
            var model = new RoomViewModel();
            model.CompanyId = "086FBDC2-14F3-4F68-B3C6-9EA42D257061";
            model.OperatorName = "John Dou";

            return View(model);
        }
    }
}