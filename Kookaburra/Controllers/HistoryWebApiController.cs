using AutoMapper;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.ChatHistory;
using Kookaburra.Domain.Query.ChatHistorySearch;
using Kookaburra.Models.History;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class HistoryWebApiController : ApiController
    {
        private readonly IQueryHandler<ChatHistoryQuery, Task<ChatHistoryQueryResult>> _chatHistoryQueryHandler;
        private readonly IQueryHandler<ChatHistorySearchQuery, Task<ChatHistoryQueryResult>> _chatHistorySearchQueryHandler;
        
        private readonly int PageSize = 5;


        public HistoryWebApiController(IQueryHandler<ChatHistoryQuery, Task<ChatHistoryQueryResult>> chatHistoryQueryHandler,
            IQueryHandler<ChatHistorySearchQuery, Task<ChatHistoryQueryResult>> chatHistorySearchQueryHandler)
        {
            _chatHistoryQueryHandler = chatHistoryQueryHandler;
            _chatHistorySearchQueryHandler = chatHistorySearchQueryHandler;
        }

        [HttpGet, Route("api/history/{filter}/{page}")]
        public async Task<ChatHistoryViewModel> LoadMore(string filter, int page)
        {
            TimeFilterType timeFilter = TimeFilterType.All;
            Enum.TryParse(filter, out timeFilter);

            var query = new ChatHistoryQuery(timeFilter, User.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, page)
            };
            var result = await _chatHistoryQueryHandler.ExecuteAsync(query);

            var viewModel = Mapper.Map<ChatHistoryViewModel>(result);

            return viewModel;
        }

        [HttpGet, Route("api/history/search/{queryTerm}/{page}")]
        public async Task<ChatHistoryViewModel> Search(string queryTerm, int page)
        {
            var query = new ChatHistorySearchQuery(queryTerm, User.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, page)
            };
            var result = await _chatHistorySearchQueryHandler.ExecuteAsync(query);

            var viewModel = Mapper.Map<ChatHistoryViewModel>(result);

            return viewModel;
        }
    }
}