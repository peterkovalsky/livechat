using System.Web.Mvc;


namespace Kookaburra.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        [HttpGet]
        [Route("settings/code")]
        public ActionResult Code()
        {
            return View();
        }
    }
}