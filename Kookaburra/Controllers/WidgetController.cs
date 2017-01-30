using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Models.Widget;
using Kookaburra.ViewModels.Widget;
using System;
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
        public ActionResult Container(string key)
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
        [Route("widget/{key}")]
        public ActionResult Widget(string key)
        {
            var operatorResult = _queryDispatcher.Execute<AvailableOperatorQuery, AvailableOperatorQueryResult>(new AvailableOperatorQuery(key));
            var sessionId = Session[COOKIE_SESSION_ID];

            if (operatorResult != null) // There is someone online
            {
                if (sessionId != null)
                {
                    var query = new CurrentSessionQuery { VisitorSessionId = sessionId.ToString() };
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
        [Route("widget/introduction")]
        [ValidateAntiForgeryToken]      
        public ActionResult Introduction(IntroductionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // get location
            var location = "Sydney, Australia";
            var availableOperator = _queryDispatcher.Execute<AvailableOperatorQuery, AvailableOperatorQueryResult>(new AvailableOperatorQuery(model.AccountKey));
            
            // if operator is available - establish connection
            if (availableOperator != null)
            {
                var sessionId = Guid.NewGuid().ToString();

                var command = new StartConversationCommand(availableOperator.OperatorConnectionId, model.Name, sessionId);
                command.Page = model.PageUrl;
                command.Location = location;
                command.VisitorEmail = model.Email;

                _commandDispatcher.Execute(command);
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
        [Route("widget/offline/{key}")]
        public ActionResult Offline(string key)
        {
            var model = new OfflineViewModel { AccountKey = key };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("widget/offline")]
        public ActionResult Offline(OfflineViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var command = new LeaveMessageCommand(model.AccountKey, model.Name, model.Email, model.Message);
            _commandDispatcher.Execute(command);

            return RedirectToAction("ThankYou");
        }

        [HttpGet]
        [Route("widget/thankyou")]
        public ActionResult ThankYou()
        {
            return View();
        }
    }
}