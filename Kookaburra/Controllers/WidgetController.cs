﻿using System.Web.Mvc;
using Kookaburra.ViewModels.Widget;

namespace Kookaburra.Controllers
{
    public class WidgetController : Controller
    {
        [HttpGet]
        [Route("chatbox/{key}")]
        public ActionResult ChatBox(string key)
        {
            bool isOnline = true;

            if (isOnline)
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
        public ActionResult Offline(OfflineBoxViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            //var webFolder = Server.MapPath("~");

            //BackgroundJob.Enqueue(() => SendAndLogMesssage(model, webFolder));

            return Content("<div class=\"heading\">Thank you! Your message has been sent. We will get back to you as soon as we can.</div>", "text/html");
        }

        [HttpGet]
        [Route("chatboxstyle/{key}")]
        public ActionResult ChatBoxStyle(string key)
        {
            var model = new ChatBoxStyleViewModel { Key = key };

            model.ChatServerHost = "http://local.kookaburra.com";

            HttpContext.Response.ContentType = "application/javascript";

            return View(model);
        }
    }
}