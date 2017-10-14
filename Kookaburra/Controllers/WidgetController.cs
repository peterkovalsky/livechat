using Kookaburra.Common;
using Kookaburra.Models.Widget;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class WidgetController : Controller
    {
        [HttpGet]
        [Route("widget/{key}")]
        public ActionResult ContainerJS(string key)
        {
            var model = new ContainerViewModel
            {
                AccountKey = key,
                ChatServerHost = WebHelper.Host
            };

            HttpContext.Response.ContentType = "application/javascript";

            return View(model);
        }

        [HttpGet]
        [Route("widget/default/{key}")]
        public ActionResult Widget(string key)
        {
            ViewBag.AccountKey = key;

            return View();
        }
    }
}