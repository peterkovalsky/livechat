using Kookaburra.Domain.Command;
using Kookaburra.Domain.Query;
using System.Web.Mvc;


namespace Kookaburra.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private readonly int PageSize = 10;

        public HistoryController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet, Route("history")]
        public ActionResult List()
        {
            return View();
        }

        [HttpGet, Route("history/{id}")]       
        public ActionResult Chat(long id)
        {
            return View();
        }
    }
}