using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NotFound()
        {
            Response.StatusCode = 404;

            return View("Error404");
        }
    }
}