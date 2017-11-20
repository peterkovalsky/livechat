using AutoMapper;
using Kookaburra.Domain.Common;
using Kookaburra.Models.History;
using Kookaburra.Services.Chats;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class HistoryWebApiController : ApiController
    {
        private readonly IChatService _chatService;        
        private readonly int PageSize = 5;

        public HistoryWebApiController(IChatService chatService)
        {
            _chatService = chatService;
        }


        [HttpGet, Route("api/history/{filter}/{page}")]
        public async Task<ChatHistoryViewModel> LoadMore(string filter, int page)
        {
            TimeFilterType timeFilter = TimeFilterType.All;
            Enum.TryParse(filter, out timeFilter);
            
            var result = await _chatService.GetChatHistoryAsync(timeFilter, User.Identity.GetUserId(), new Pagination(PageSize, page));
            var viewModel = Mapper.Map<ChatHistoryViewModel>(result);

            return viewModel;
        }

        [HttpGet, Route("api/history/search/{queryTerm}/{page}")]
        public async Task<ChatHistoryViewModel> Search(string queryTerm, int page)
        {
            var result = await _chatService.SearchChatHistoryAsync(queryTerm, User.Identity.GetUserId(), new Pagination(PageSize, page));
            var viewModel = Mapper.Map<ChatHistoryViewModel>(result);

            return viewModel;
        }
    }
}