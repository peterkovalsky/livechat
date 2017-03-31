using AutoMapper;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Models.Offline;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class OfflineController : Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private readonly int PageSize = 5;

        public OfflineController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet, Route("messages")]
        public ActionResult Messages()
        {
            var query = new OfflineMessagesQuery(TimeFilterType.All, User.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, 1)
            };
            var result = _queryDispatcher.Execute<OfflineMessagesQuery, OfflineMessagesQueryResult>(query);

            var viewModel = Mapper.Map<OfflineMessagesViewModel>(result);
            viewModel.PageSize = PageSize;

            return View(viewModel);
        }       
    }
}