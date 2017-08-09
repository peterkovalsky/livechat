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
using Microsoft.AspNet.Identity.Owin;
using System.Web;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class WebAPIController : ApiController
    {
        private readonly ICommandHandler<MarkMessageAsReadCommand> _markMessageAsReadCommandHandler;
        private readonly ICommandHandler<DeleteMessageCommand> _deleteMessageCommandHandler;

        private readonly IQueryHandler<OfflineMessagesQuery, Task<OfflineMessagesQueryResult>> _offlineMessagesQueryHandler;
        private readonly IQueryHandler<SearchOfflineMessagesQuery, Task<OfflineMessagesQueryResult>> _searchOfflineMessagesQueryHandler;

        private ApplicationUserManager _userManager;

        private readonly int PageSize = 10;  

        public WebAPIController(
            ICommandHandler<MarkMessageAsReadCommand> markMessageAsReadCommandHandler,
            ICommandHandler<DeleteMessageCommand> deleteMessageCommandHandler,
            IQueryHandler<OfflineMessagesQuery, Task<OfflineMessagesQueryResult>> offlineMessagesQueryHandler,
            IQueryHandler<SearchOfflineMessagesQuery, Task<OfflineMessagesQueryResult>> searchOfflineMessagesQueryHandler)
        {
            _markMessageAsReadCommandHandler = markMessageAsReadCommandHandler;
            _deleteMessageCommandHandler = deleteMessageCommandHandler;

            _offlineMessagesQueryHandler = offlineMessagesQueryHandler;
            _searchOfflineMessagesQueryHandler = searchOfflineMessagesQueryHandler;

            _userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        }
      

        [HttpGet, Route("api/messages/{filter}/{page}")]
        public async Task<OfflineMessagesViewModel> GetMoreMessages(string filter, int page)
        {
            TimeFilterType timeFilter = TimeFilterType.All;         
            Enum.TryParse(filter, out timeFilter);

            var user = await _userManager.FindByIdAsync(RequestContext.Principal.Identity.GetUserId());

            var query = new OfflineMessagesQuery(timeFilter, user.AccountKey)
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
            var user = await _userManager.FindByIdAsync(RequestContext.Principal.Identity.GetUserId());

            var query = new SearchOfflineMessagesQuery(queryTerm, user.AccountKey)
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
            var user = await _userManager.FindByIdAsync(RequestContext.Principal.Identity.GetUserId());

            var command = new MarkMessageAsReadCommand(id, user.AccountKey);
            await _markMessageAsReadCommandHandler.ExecuteAsync(command);
        }

        [HttpDelete, Route("api/messages/{id}")]
        public async Task DeleteMessage(int id)
        {
            var user = await _userManager.FindByIdAsync(RequestContext.Principal.Identity.GetUserId());

            var command = new DeleteMessageCommand(id, user.AccountKey);
            await _deleteMessageCommandHandler.ExecuteAsync(command);
        }
    }
}