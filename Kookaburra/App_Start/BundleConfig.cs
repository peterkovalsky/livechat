using System.Web.Optimization;

namespace Kookaburra
{
    public class BundleConfig
    {       
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region JS

            bundles.Add(new ScriptBundle("~/bundles/js/widget")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.signalR-{version}.js")
                .Include("~/Scripts/knockout-{version}.js")
                .Include("~/Scripts/chat-widget.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));         

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            #endregion

            #region CSS

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            #endregion
        }
    }
}