using System.Web.Optimization;

namespace Kookaburra
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region JS

            bundles.Add(new ScriptBundle("~/bundles/js/form")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.validate*")
            );

            bundles.Add(new ScriptBundle("~/bundles/js/widget")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.signalR-{version}.js")
                .Include("~/Scripts/knockout-{version}.js")
                .Include("~/Scripts/chat/widget.js")
                .Include("~/Scripts/moment-with-locales.min.js")
                .Include("~/Scripts/perfect-scrollbar.jquery.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/js/chatroom")        
                .Include("~/Scripts/moment-with-locales.min.js")
                .Include("~/Scripts/chat/chat-room.js")
                //.Include("~/Scripts/perfect-scrollbar.jquery.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/portal")
                //.Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.signalR-{version}.js")
                .Include("~/Scripts/knockout-{version}.js")
                .Include("~/Scripts/knockout.mapping-latest.js")                
                .Include("~/Scripts/chat/operator-global.js")              
            );

            #endregion


            #region CSS

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/bundles/css/chatroom")
                //.Include("~/Content/perfect-scrollbar.css")
            );

            bundles.Add(new StyleBundle("~/bundles/css/widget")
                .Include("~/Content/widget.css")
                .Include("~/Content/perfect-scrollbar.css")
            );

            #endregion
        }
    }
}