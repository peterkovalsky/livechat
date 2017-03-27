﻿using AutoMapper;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Models.Offline;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class WebAPIController : ApiController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private readonly int PageSize = 20;

        public WebAPIController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }
      
        [HttpGet, Route("api/messages/{page}")]
        public List<LeftMessageViewModel> GetMoreMessages(int page)
        {
            var query = new OfflineMessagesQuery(TimeFilterType.All)
            {
                Pagination = new Pagination(PageSize, page)
            };
            var result = _queryDispatcher.Execute<OfflineMessagesQuery, OfflineMessagesQueryResult>(query);

            var viewModel = result.OfflineMessages.Select(om => Mapper.Map<LeftMessageViewModel>(om)).ToList();

            return viewModel;
        }
    }
}