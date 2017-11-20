using AutoMapper;
using Kookaburra.Domain.Common;
using Kookaburra.Models.History;
using Kookaburra.Services.Chats;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace Kookaburra.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {        
        private readonly IChatService _chatService;

        private readonly int PageSize = 10;

        public HistoryController(IChatService chatService)
        {       
            _chatService = chatService;
        }


        [HttpGet, Route("history")]
        public async Task<ActionResult> Chats()
        {
            var result = await _chatService.GetChatHistoryAsync(TimeFilterType.All, User.Identity.GetUserId(), new Pagination(PageSize, 1));
            var viewModel = Mapper.Map<ChatHistoryViewModel>(result);            

            return View(viewModel);
        }

        [HttpGet, Route("transcript/{id}")]       
        public async Task<ActionResult> Transcript(long id)
        {
            var result = await _chatService.GetTranscriptAsync(id, User.Identity.GetUserId());
            if (result == null)
            {
                return HttpNotFound();
            }

            var viewModel = Mapper.Map<TranscriptViewModel>(result);

            return View(viewModel);
        }
    }
}