using AutoMapper;
using Kookaburra.Domain.Common;
using Kookaburra.Models.Offline;
using Kookaburra.Services.OfflineMessages;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Kookaburra.Controllers
{
    [Authorize]
    public class WebAPIController : ApiController
    {
        private readonly int PageSize = 10;

        private readonly IOfflineMessageService _offlineMessageService;

        public WebAPIController(IOfflineMessageService offlineMessageService)
        {
            _offlineMessageService = offlineMessageService;
        }
      

        [HttpGet, Route("api/messages/{filter}/{page}")]
        public async Task<OfflineMessagesViewModel> GetMoreMessages(string filter, int page)
        {
            TimeFilterType timeFilter = TimeFilterType.All;         
            Enum.TryParse(filter, out timeFilter);      

            var pagination = new Pagination(PageSize, page);
            var result = await _offlineMessageService.GetOfflineMessagesAsync(timeFilter, RequestContext.Principal.Identity.GetUserId(), pagination);         

            return new OfflineMessagesViewModel
            {
                OfflineMessages = Mapper.Map<List<LeftMessageViewModel>>(result),
                PageSize = pagination.Size,
                TotalMessages = pagination.Total
            };
        }

        [HttpGet, Route("api/messages/search/{queryTerm}/{page}")]
        public async Task<OfflineMessagesViewModel> SearchMessages(string queryTerm, int page)
        {
            var pagination = new Pagination(PageSize, page);
            var result = await _offlineMessageService.SearchOfflineMessagesAsync(queryTerm, RequestContext.Principal.Identity.GetUserId(), pagination);

            return new OfflineMessagesViewModel
            {
                OfflineMessages = Mapper.Map<List<LeftMessageViewModel>>(result),
                PageSize = pagination.Size,
                TotalMessages = pagination.Total
            };
        }

        [HttpPatch, Route("api/messages/mark-read/{id}")]
        public async Task MarkMessageAsRead(long id)
        {
            await _offlineMessageService.MarkMessageAsReadAsync(id, RequestContext.Principal.Identity.GetUserId());
        }

        [HttpDelete, Route("api/messages/{id}")]
        public async Task DeleteMessage(int id)
        {            
            await _offlineMessageService.DeleteMessageAsync(id, RequestContext.Principal.Identity.GetUserId());
        }
    }
}