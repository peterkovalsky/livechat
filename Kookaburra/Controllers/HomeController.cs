using AutoMapper;
using Kookaburra.Common;
using Kookaburra.Domain.Common;
using Kookaburra.Models.Account;
using Kookaburra.Models.Home;
using Kookaburra.Services.Accounts;
using Kookaburra.Services.Chats;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
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
        private readonly IChatService _chatService;

        public HomeController(IAccountService accountService, IChatService chatService)
        {
            _accountService = accountService;
            _chatService = chatService;
        }


        [HttpGet]
        [Route("")]
        [Route("dashboard")]
        public async Task<ActionResult> Dashboard()
        {
            var currentOperator = await _accountService.GetOperatorAsync(User.Identity.GetUserId());
            var chatsPerDay = await _chatService.GetChatsPerDay(User.Identity.GetUserId(), 30);

            var model = new DashboardViewModel
            {
                AccountStatus = currentOperator.Account.AccountStatus,
                TrialDaysLeft = currentOperator.Account.TrialDaysLeft,
                TotalChats = chatsPerDay.Select(c => c.TotalChats).Sum(),
                TrialExpiredViewModel = new TrialExpiredViewModel
                {
                    Name = currentOperator.FirstName,
                    TrialPeriodDays = currentOperator.Account.TrialPeriodDays,
                },
                DailyChats = chatsPerDay.Select(c => new DailyChatsViewModel
                {
                    Day = c.Day.JsDateTime(),
                    TotalChats = c.TotalChats
                }).ToList()
            };

            return View(model);
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