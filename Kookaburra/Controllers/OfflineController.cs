using Kookaburra.Models.Offline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class OfflineController : Controller
    {
        [HttpGet, Route("messages")]
        public ActionResult Messages()
        {
            var messages = new List<MessagesViewModel>()
            {
                
            };

            return View();
        }
    }
}