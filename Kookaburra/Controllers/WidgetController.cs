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
                        return View("Online", new OnlineBoxViewModel { AccountKey = key });
                    }

                    // Introduction                 
                    return View("Introduction", new IntroductionViewModel { AccountKey = key });
                }

                // Introduction                 
                return View("Introduction", new IntroductionViewModel { AccountKey = key });
            }

            // Offline - no operator available
            return View("Offline", new OfflineViewModel { AccountKey = key });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]      
        public ActionResult Introduction(IntroductionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var command = new StartConversationCommand(operatorResult.OperatorConnectionId, Context.ConnectionId, name, sessionId);
            command.Page = page;
            command.Location = location;
            command.VisitorEmail = email;

            _commandDispatcher.Execute(command);

            return View("Online", new OnlineBoxViewModel { AccountKey = model.AccountKey });
        }

        [HttpGet]
        [Route("widget/offline/{key}")]
        public ActionResult Offline(string key)
        {
            var model = new OfflineViewModel { AccountKey = key };

            return View("Offline", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("widget/offline")]
        public ActionResult Offline(OfflineViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var command = new LeaveMessageCommand(model.AccountKey, model.Name, model.Email, model.Message);
            _commandDispatcher.Execute(command);

            return View("ThankYou");
        }
    }
}