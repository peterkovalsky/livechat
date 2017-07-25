using AutoMapper;
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
        private readonly ICommandHandler<MarkMessageAsReadCommand> _markMessageAsReadCommandHandler;
        private readonly ICommandHandler<DeleteMessageCommand> _deleteMessageCommandHandler;

        private readonly IQueryHandler<OfflineMessagesQuery, Task<OfflineMessagesQueryResult>> _offlineMessagesQueryHandler;
        private readonly IQueryHandler<SearchOfflineMessagesQuery, Task<OfflineMessagesQueryResult>> _searchOfflineMessagesQueryHandler;
        
        private readonly int PageSize = 10;  

        public WebAPIController(
            ICommandHandler<MarkMessageAsReadCommand> markMessageAsReadCommandHandler,
            ICommandHandler<DeleteMessageCommand> deleteMessageCommandHandler,
            IQueryHandler<OfflineMessagesQuery, Task<OfflineMessagesQueryResult>> offlineMessagesQueryHandler,
            IQueryHandler<SearchOfflineMessagesQuery, Task<OfflineMessagesQueryResult>> searchOfflineMessagesQueryHandler
        )
        {
            _markMessageAsReadCommandHandler = markMessageAsReadCommandHandler;
            _deleteMessageCommandHandler = deleteMessageCommandHandler;

            _offlineMessagesQueryHandler = offlineMessagesQueryHandler;
            _searchOfflineMessagesQueryHandler = searchOfflineMessagesQueryHandler;
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
            var result = await _offlineMessagesQueryHandler.ExecuteAsync(query);

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
            var result = await _searchOfflineMessagesQueryHandler.ExecuteAsync(query);
            
            var viewModel = Mapper.Map<OfflineMessagesViewModel>(result);

            return viewModel;
        }

        [HttpPatch, Route("api/messages/mark-read/{id}")]
        public async Task MarkMessageAsRead(int id)
        {            
            var command = new MarkMessageAsReadCommand(id, RequestContext.Principal.Identity.GetUserId());
            await _markMessageAsReadCommandHandler.ExecuteAsync(command);
        }

        [HttpDelete, Route("api/messages/{id}")]
        public async Task DeleteMessage(int id)
        {
            var command = new DeleteMessageCommand(id, RequestContext.Principal.Identity.GetUserId());
            await _deleteMessageCommandHandler.ExecuteAsync(command);
        }
    }
}