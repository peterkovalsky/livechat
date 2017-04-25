using AutoMapper;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.ChatHistory;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Models.History;
using Microsoft.AspNet.Identity;
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
            var query = new ChatHistoryQuery(TimeFilterType.All, User.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, 1)
            };
            var result = _queryDispatcher.Execute<ChatHistoryQuery, ChatHistoryQueryResult>(query);

            var viewModel = Mapper.Map<ChatHistoryViewModel>(result);            

            return View(viewModel);
        }

        [HttpGet, Route("transcript/{id}")]       
        public ActionResult Transcript(long id)
        {
            var query = new TranscriptQuery(id, User.Identity.GetUserId());
            var result = _queryDispatcher.Execute<TranscriptQuery, TranscriptQueryResult>(query);

            if (result == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<TranscriptViewModel>(result);

            return View(viewModel);
        }
    }
}