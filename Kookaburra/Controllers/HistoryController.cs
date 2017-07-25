using AutoMapper;
using Kookaburra.Domain.Common;
using Kookaburra.Domain.Query;
using Kookaburra.Domain.Query.ChatHistory;
using Kookaburra.Domain.Query.Transcript;
using Kookaburra.Models.History;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace Kookaburra.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly IQueryHandler<ChatHistoryQuery, Task<ChatHistoryQueryResult>> _chatHistoryQueryHandler;
        private readonly IQueryHandler<TranscriptQuery, Task<TranscriptQueryResult>> _transcriptQueryHandler;
        
        private readonly int PageSize = 10;

        public HistoryController(IQueryHandler<ChatHistoryQuery, Task<ChatHistoryQueryResult>> chatHistoryQueryHandler,
            IQueryHandler<TranscriptQuery, Task<TranscriptQueryResult>> transcriptQueryHandler)
        {
            _chatHistoryQueryHandler = chatHistoryQueryHandler;
            _transcriptQueryHandler = transcriptQueryHandler;
        }

        [HttpGet, Route("history")]
        public async Task<ActionResult> Chats()
        {
            var query = new ChatHistoryQuery(TimeFilterType.All, User.Identity.GetUserId())
            {
                Pagination = new Pagination(PageSize, 1)
            };
            var result = await _chatHistoryQueryHandler.ExecuteAsync(query);

            var viewModel = Mapper.Map<ChatHistoryViewModel>(result);            

            return View(viewModel);
        }

        [HttpGet, Route("transcript/{id}")]       
        public async Task<ActionResult> Transcript(long id)
        {
            var query = new TranscriptQuery(id, User.Identity.GetUserId());
            var result = await _transcriptQueryHandler.ExecuteAsync(query);

            if (result == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<TranscriptViewModel>(result);

            return View(viewModel);
        }
    }
}