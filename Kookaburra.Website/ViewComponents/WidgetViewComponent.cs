using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Kookaburra.Website.ViewComponents
{
    public class WidgetViewComponent : ViewComponent
    {
        private readonly SiteConfigs _siteConfigs;

        public WidgetViewComponent(IOptions<SiteConfigs> optionsAccessor)
        {
            _siteConfigs = optionsAccessor.Value;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["WidgetHost"] = _siteConfigs?.Widget?.Host;

            return View();
        }
    }
}