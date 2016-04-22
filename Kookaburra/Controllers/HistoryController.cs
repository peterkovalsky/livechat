using System.Web.Mvc;


namespace Kookaburra.Controllers
{
    public class HistoryController : Controller
    {
        [HttpGet]
        [Route("history")]
        public ActionResult List()
        {
            return View();
        }
    }
}