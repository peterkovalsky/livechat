using AutoMapper;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Models.History;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class HistoryWebApiController : ApiController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private readonly int PageSize = 5;


        public HistoryWebApiController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet, Route("api/history/{filter}/{page}")]
        public ChatHistoryViewModel LoadMore(string filter, int page)
        {
            TimeFilterType timeFilter = TimeFilterType.All;
            Enum.TryParse(filter, out timeFilter);

            var query = new ChatHistoryQuery(timeFilter, User.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, page)
            };
            var result = _queryDispatcher.Execute<ChatHistoryQuery, ChatHistoryQueryResult>(query);

            var viewModel = Mapper.Map<ChatHistoryViewModel>(result);

            return viewModel;
        }
    }
}