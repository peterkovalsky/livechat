using Kookaburra.Domain.Repository;
using Kookaburra.Repository;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;

namespace Kookaburra
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
        }

        protected void Application_AuthorizeRequest(Object sender, EventArgs e)
        {
            // We only want to run the code for authenticated users
            // As this is an MVC project we also only want to run the code for action requests         
            if (HttpContext.Current.User.Identity.IsAuthenticated
                && RouteTable.Routes.GetRouteData(new HttpContextWrapper(HttpContext.Current)) != null)
            {
                // Create the user activity repository
                IOperatorRepository operatorRepository = new OperatorRepository(new KookaburraContext("name=DefaultConnection"));     
                
                operatorRepository.RecordOperatorActivity(HttpContext.Current.User.Identity.GetUserId());
            }
        }
    }
}