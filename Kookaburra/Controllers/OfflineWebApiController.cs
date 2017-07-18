﻿using AutoMapper;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.DeleteMessage;
using Kookaburra.Domain.Command.MarkMessageAsRead;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.OfflineMessages;
using Kookaburra.Domain.Query.SearchOfflineMessages;
using Kookaburra.Models.Offline;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class WebAPIController : ApiController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        private readonly int PageSize = 10;
  

        public WebAPIController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }
      
        [HttpGet, Route("api/messages/{filter}/{page}")]
        public async Task<OfflineMessagesViewModel> GetMoreMessages(string filter, int page)
        {
            TimeFilterType timeFilter = TimeFilterType.All;         
            Enum.TryParse(filter, out timeFilter);            

            var query = new OfflineMessagesQuery(timeFilter, RequestContext.Principal.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, page)
            };
            var result = await _queryDispatcher.ExecuteAsync<OfflineMessagesQuery, Task<OfflineMessagesQueryResult>>(query);

            var viewModel = Mapper.Map<OfflineMessagesViewModel>(result);

            return viewModel;
        }

        [HttpGet, Route("api/messages/search/{queryTerm}/{page}")]
        public async Task<OfflineMessagesViewModel> SearchMessages(string queryTerm, int page)
        {
            var query = new SearchOfflineMessagesQuery(queryTerm, RequestContext.Principal.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, page)
            };
            var result = await _queryDispatcher.ExecuteAsync<SearchOfflineMessagesQuery, Task<OfflineMessagesQueryResult>>(query);
            
            var viewModel = Mapper.Map<OfflineMessagesViewModel>(result);

            return viewModel;
        }

        [HttpPatch, Route("api/messages/mark-read/{id}")]
        public async Task MarkMessageAsRead(int id)
        {            
            var command = new MarkMessageAsReadCommand(id, RequestContext.Principal.Identity.GetUserId());
            await _commandDispatcher.ExecuteAsync(command);
        }

        [HttpDelete, Route("api/messages/{id}")]
        public async Task DeleteMessage(int id)
        {
            var command = new DeleteMessageCommand(id, RequestContext.Principal.Identity.GetUserId());
            await _commandDispatcher.ExecuteAsync(command);
        }
    }
}