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
            if (Request.Host.Host.Equals("staging.kookaburra.chat"))
            {
                ViewData["WidgetHost"] = _siteConfigs.Widget.StagingHost;
            }
            else if (Request.Host.Host.Equals("kookaburra.chat"))
            {
                ViewData["WidgetHost"] = _siteConfigs.Widget.ProductionHost;
            }
            else
            {
                ViewData["WidgetHost"] = _siteConfigs.Widget.LocalHost;
            }

            return View();
        }
    }
}