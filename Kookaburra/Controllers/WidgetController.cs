using Kookaburra.Services;
using Kookaburra.ViewModels.Widget;
using System;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class WidgetController : Controller
    {
        private readonly ChatSession _chatSession;

        public WidgetController(ChatSession chatSession)
        {
            _chatSession = chatSession;
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
            if (_chatSession.AnyOperatorAvailable(key)) // There is someone online
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
        [Route("chatbox/offline/{key}")]
        public ActionResult Offline(OfflineBoxViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            //var webFolder = Server.MapPath("~");

            //BackgroundJob.Enqueue(() => SendAndLogMesssage(model, webFolder));

            return Content("<div class=\"heading\">Thank you! Your message has been sent. We will get back to you as soon as we can.</div>", "text/html");
        }    
    }
}