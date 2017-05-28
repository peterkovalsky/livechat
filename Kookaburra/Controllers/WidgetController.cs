using Kookaburra.Common;
using Kookaburra.Domain.AvailableOperator;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Command.StartVisitorChat;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.CurrentSession;
using Kookaburra.Models.Widget;
using Kookaburra.ViewModels.Widget;
using Microsoft.AspNet.Identity;
using SimpleHoneypot.ActionFilters;
using System;
using System.Web;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class WidgetController : Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;       

        public const string COOKIE_SESSION_ID = "kookaburra.visitor.sessionid";

        public WidgetController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;           
        }


        [HttpGet]
        [Route("widget/container/{key}")]
        public ActionResult ContainerJS(string key)
        {
            var model = new ContainerViewModel
            {
                AccountKey = key,
                ChatServerHost = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + ":" + Request.Url.Port
            };

            HttpContext.Response.ContentType = "application/javascript";

            return View(model);
        }

        [HttpGet]
        [Route("widget/light/{key}")]
        public ActionResult Widget(string key)
        {
            ViewBag.AccountKey = key;

            return View();
        }

        [HttpGet]
        [Route("widget/{key}")]
        public ActionResult WidgetOld(string key)
        {
            var operatorResult = _queryDispatcher.Execute<AvailableOperatorQuery, AvailableOperatorQueryResult>(new AvailableOperatorQuery(key));
            var sessionId = Request.Cookies[COOKIE_SESSION_ID];

            if (operatorResult != null) // There is someone online
            {
                if (sessionId != null && !string.IsNullOrWhiteSpace(sessionId.Value))
                {
                    var query = new CurrentSessionQuery(User.Identity.GetUserId()) { VisitorSessionId = sessionId.Value };
                    var currentSession = _queryDispatcher.Execute<CurrentSessionQuery, CurrentSessionQueryResult>(query);

                    if (currentSession != null)
                    {
                        // Online - resume chat
                        return View(nameof(WidgetController.Online), new OnlineBoxViewModel { AccountKey = key });
                    }

                    // Introduction                 
                    return View(nameof(WidgetController.Introduction), new IntroductionViewModel { AccountKey = key });
                }

                // Introduction                 
                return View(nameof(WidgetController.Introduction), new IntroductionViewModel { AccountKey = key });
            }

            // Offline - no operator available
            return View(nameof(WidgetController.Offline), new OfflineViewModel { AccountKey = key });
        }

        [HttpPost]
        [Honeypot]
        [Route("widget/introduction")]
        [ValidateAntiForgeryToken]      
        public ActionResult Introduction(IntroductionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
       
            var availableOperator = _queryDispatcher.Execute<AvailableOperatorQuery, AvailableOperatorQueryResult>(new AvailableOperatorQuery(model.AccountKey));
            
            // if operator is available - establish connection
            if (availableOperator != null)
            {
                var sessionId = Guid.NewGuid().ToString();

                var command = new StartVisitorChatCommand(availableOperator.OperatorId, model.Name, sessionId, User.Identity.GetUserId())
                {
                    Page = model.PageUrl,
                    VisitorIP = WebHelper.GetIPAddress(),
                    VisitorEmail = model.Email
                };
                _commandDispatcher.Execute(command);

                // add sessionId cookie
                Response.Cookies.Set(new HttpCookie(COOKIE_SESSION_ID, sessionId));
            }

            return RedirectToAction("Online", new { key = model.AccountKey });
        }

        [HttpGet]
        [Route("widget/online/{key}")]
        public ActionResult Online(string key)
        {
            var model = new OnlineBoxViewModel { AccountKey = key };

            return View(model);
        }

        [HttpGet]
        [Route("widget/stop")]
        public ActionResult Stop(string key)
        {
            var model = new ThankYouViewModel { ThankYouText = "Thank you for chatting with us!" };

            // remove sessionId cookie
            Response.Cookies.Set(new HttpCookie(COOKIE_SESSION_ID, null));

            return View(nameof(WidgetController.ThankYou), model);
        }

        [HttpGet]
        [Route("widget/offline/{key}")]
        public ActionResult Offline(string key)
        {
            var model = new OfflineViewModel { AccountKey = key };

            return View(model);
        }

        [HttpPost]
        [Honeypot]
        [ValidateAntiForgeryToken]
        [Route("widget/offline")]
        public ActionResult Offline(OfflineViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var command = new LeaveMessageCommand(model.AccountKey, model.Name, model.Email, model.Message, User.Identity.GetUserId())
            {
                VisitorIP = WebHelper.GetIPAddress()
            };
            _commandDispatcher.Execute(command);

            return RedirectToAction(nameof(WidgetController.ThankYou));
        }

        [HttpGet]
        [Route("widget/thankyou")]
        public ActionResult ThankYou()
        {
            var model = new ThankYouViewModel { ThankYouText = "Thank you! Your message has been sent. We will get back to you as soon as we can." };

            return View(model);
        }
    }
}