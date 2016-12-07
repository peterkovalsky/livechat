using Kookaburra.Domain.Command;
using Kookaburra.Domain.Model;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.ViewModels.Widget;
using System;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class WidgetController : Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public WidgetController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }


        [HttpGet]
        [Route("chatbox/{key}")]
        public ActionResult ChatBox(string key)
        {
            var model = new ChatBoxStyleViewModel { Key = key };

            model.ChatServerHost = Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host + ":" + Request.Url.Port;

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

            var model = new OfflineBoxViewModel { AccountKey = key };

            return View("Offline", model);
        }

        [HttpGet]
        [Route("chatbox/offline/{key}")]
        public ActionResult Offline(string key)
        {    
            var model = new OfflineBoxViewModel { AccountKey = key };

            return View("Offline", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("chatbox/offline")]
        public ActionResult Offline(OfflineBoxViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var account = _accountRepository.Get(model.AccountKey);

            if (account != null)
            {
                var visitor = new Visitor {
                    Name = model.Name,
                    Email = model.Email,
                    ConversationStarted = DateTime.UtcNow
                };

                _visitorRepository.AddVisitor(visitor);

                var offlineMessage = new OfflineMessage
                {
                    Message = model.Message,
                    DateSent = DateTime.UtcNow,
                    IsRead = false,
                    AccountId = account.Id,
                    VisitorId = visitor.Id
                };

                _messageRepository.AddOfflineMessage(offlineMessage);
            }

            model.ThankYou = true;

            return View("Offline", model);
        }    
    }
}