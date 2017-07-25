using AutoMapper;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.OfflineMessages;
using Kookaburra.Models.Offline;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kookaburra.Controllers
{
    public class OfflineController : Controller
    {
        private readonly IQueryHandler<OfflineMessagesQuery, Task<OfflineMessagesQueryResult>> _offlineMessagesQuery;

        private readonly int PageSize = 10;

        public OfflineController(IQueryHandler<OfflineMessagesQuery, Task<OfflineMessagesQueryResult>> offlineMessagesQuery)
        {
            _offlineMessagesQuery = offlineMessagesQuery;
        }


        [HttpGet, Route("messages")]
        public async Task<ActionResult> Messages()
        {
            var query = new OfflineMessagesQuery(TimeFilterType.All, User.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, 1)
            };
            var result = await _offlineMessagesQuery.ExecuteAsync(query);

            var viewModel = Mapper.Map<OfflineMessagesViewModel>(result);
            viewModel.PageSize = PageSize;

            return View(viewModel);
        }       
    }
}