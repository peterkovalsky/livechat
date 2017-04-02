using AutoMapper;
using Kookaburra.Domain.Command;
using Kookaburra.Domain.Command.Model;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.Model;
using Kookaburra.Domain.Query.Result;
using Kookaburra.Models.Offline;
using Microsoft.AspNet.Identity;
using System;
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

        private readonly int PageSize = 5;
  

        public WebAPIController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }
      
        [HttpGet, Route("api/messages/{filter}/{page}")]
        public OfflineMessagesViewModel GetMoreMessages(string filter, int page)
        {
            TimeFilterType timeFilter = TimeFilterType.All;         
            Enum.TryParse(filter, out timeFilter);            

            var query = new OfflineMessagesQuery(timeFilter, RequestContext.Principal.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, page)
            };
            var result = _queryDispatcher.Execute<OfflineMessagesQuery, OfflineMessagesQueryResult>(query);

            var viewModel = Mapper.Map<OfflineMessagesViewModel>(result);

            return viewModel;
        }

        [HttpGet, Route("api/messages/search/{queryTerm}/{page}")]
        public OfflineMessagesViewModel SearchMessages(string queryTerm, int page)
        {
            var query = new SearchOfflineMessagesQuery(queryTerm, RequestContext.Principal.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, page)
            };
            var result = _queryDispatcher.Execute<SearchOfflineMessagesQuery, OfflineMessagesQueryResult>(query);
            
            var viewModel = Mapper.Map<OfflineMessagesViewModel>(result);

            return viewModel;
        }

        [HttpPatch, Route("api/messages/mark-read/{id}")]
        public void MarkMessageAsRead(int id)
        {            
            var command = new MarkMessageAsReadCommand(id, RequestContext.Principal.Identity.GetUserId());
            _commandDispatcher.Execute(command);
        }

        [HttpDelete, Route("api/messages/{id}")]
        public void DeleteMessage(int id)
        {
            var command = new DeleteMessageCommand(id, RequestContext.Principal.Identity.GetUserId());
            _commandDispatcher.Execute(command);
        }
    }
}