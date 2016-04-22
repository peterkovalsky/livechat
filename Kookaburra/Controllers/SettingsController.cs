using System.Web.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Kookaburra.Controllers
{
    public class SettingsController : Controller
    {
        [HttpGet]
        [Route("settings")]
        public ActionResult Index()
        {
            return View();
        }
    }
}