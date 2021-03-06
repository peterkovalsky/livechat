﻿using System.Web.Optimization;

namespace Kookaburra
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region JS

            bundles.Add(new ScriptBundle("~/bundles/js/portal")
                 //.Include("~/Scripts/jquery-{version}.js")
                 .Include("~/Scripts/jquery.signalR-{version}.js")
                 .Include("~/Scripts/knockout-{version}.js")
                 .Include("~/Scripts/knockout.mapping-latest.js")
                 .Include("~/Scripts/kookaburra/operator-global.js")
             );

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
                .Include("~/Scripts/jquery.scrollbar.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/js/widget/2")
               .Include("~/Scripts/jquery-{version}.js")
               .Include("~/Scripts/jquery.cookie.js")
               .Include("~/Scripts/jquery.signalR-{version}.js")
               .Include("~/Scripts/knockout-{version}.js")               
               .Include("~/Scripts/moment-with-locales.min.js")
               .Include("~/Scripts/jquery.scrollbar.js")
               .Include("~/Scripts/kookaburra/widget.js")
           );

            bundles.Add(new ScriptBundle("~/bundles/js/chatroom")
                .Include("~/Scripts/site.js")
                .Include("~/Scripts/moment-with-locales.min.js")
                .Include("~/Scripts/jquery.scrollbar.js")
                .Include("~/Scripts/kookaburra/chat-room.js")                
            );

            bundles.Add(new ScriptBundle("~/bundles/js/dashboard")
                .Include("~/Scripts/moment-with-locales.min.js")
                .Include("~/Scripts/chart.min.js")            
                .Include("~/Scripts/site.js")
                .Include("~/Scripts/kookaburra/dashboard.js")
         );

            bundles.Add(new ScriptBundle("~/bundles/js/offlinemessage")
                 .Include("~/Scripts/moment-with-locales.min.js")           
                 .Include("~/Scripts/kookaburra/offline-messages.js")
             );

            bundles.Add(new ScriptBundle("~/bundles/js/chathistory")
                 .Include("~/Scripts/moment-with-locales.min.js")
                 .Include("~/Scripts/kookaburra/chat-history.js")
             );

            bundles.Add(new ScriptBundle("~/bundles/js/transcript")
                .Include("~/Scripts/moment-with-locales.min.js")            
            );

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));           

            #endregion


            #region CSS

            bundles.Add(new StyleBundle("~/bundles/css/portal")
                .Include("~/remark/assets/skins/cyan.css")
                .Include("~/Content/kookaburra/portal.css")
            );

            bundles.Add(new StyleBundle("~/bundles/css/chatroom")
                .Include("~/remark/global/vendor/alertify/alertify.css")               
                .Include("~/Content/jquery.scrollbar.css")
                .Include("~/Content/kookaburra/chat-room.css")
            );

            bundles.Add(new StyleBundle("~/bundles/css/offlinemessages")
                .Include("~/remark/global/vendor/alertify/alertify.css")
                .Include("~/Content/kookaburra/offline-messages.css")
            );

            bundles.Add(new StyleBundle("~/bundles/css/chathistory")         
                .Include("~/Content/kookaburra/chat-history.css")
            );

            bundles.Add(new StyleBundle("~/bundles/css/widget")
                .Include("~/Content/kookaburra/widget.css")
                .Include("~/Content/jquery.scrollbar.css")
                .Include("~/remark/global/fonts/linecons/css/linecons.css")
            );

            #endregion
        }
    }
}