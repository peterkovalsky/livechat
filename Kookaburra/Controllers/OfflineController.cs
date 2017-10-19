using AutoMapper;
using Kookaburra.Domain.Common;
using Kookaburra.Models.Offline;
using Kookaburra.Services.OfflineMessages;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class OfflineController : Controller
    {
        private readonly IOfflineMessageService _offlineMessageService;

        private readonly int PageSize = 10;

        public OfflineController(IOfflineMessageService offlineMessageService)
        {
            _offlineMessageService = offlineMessageService;
        }


        [HttpGet, Route("messages")]
        public async Task<ActionResult> Messages()
        {                
            var result = await _offlineMessageService.GetOfflineMessagesAsync(TimeFilterType.All, User.Identity.GetUserId());
          
            return View(new OfflineMessagesViewModel
            {
                OfflineMessages = Mapper.Map<List<LeftMessageViewModel>>(result),
                PageSize = PageSize,
                TotalMessages = result.Count
            });
        }       
    }
}