using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Kookaburra.Domain.Repository;
using Kookaburra.Repository;
using Kookaburra.ViewModels.Chat;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
       
        public IAccountRepository AccountRepository { get; set; }

        public IOperatorRepository OperatorRepository { get; set; }

        private readonly ApplicationUserManager _userManager;

        public ChatController()
        {

        }

        public ChatController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
            AccountRepository = new AccountRepository(new KookaburraContext("name=DefaultConnection"));
            OperatorRepository = new OperatorRepository(new KookaburraContext("name=DefaultConnection"));
        }


        [HttpGet]
        [Route("chat-room")]
        [Route("")]
        public ActionResult Room()
        {            
            var currentOperator = OperatorRepository.Get(User.Identity.GetUserId());

            var model = new RoomViewModel();
            model.CompanyId = currentOperator.Account.Identifier;
            model.OperatorName = currentOperator.FirstName;

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