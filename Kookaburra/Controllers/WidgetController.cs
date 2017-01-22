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

            if (operatorResult != null) // There is someone online
            {
                var onlineModel = new OnlineBoxViewModel { AccountKey = key.ToUpper() };

                return View("Online", onlineModel);
            }

            var model = new OfflineViewModel { AccountKey = key };

            return View("Offline", model);
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