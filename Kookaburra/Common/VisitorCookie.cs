using System;
using System.Web;

namespace Kookaburra.Common
{
    public class VisitorCookie
    {
        public const string COOKIE_TEMPLATE = "kookaburra.visitor.{0}";

        private readonly HttpContextBase _httpContext;

        public VisitorCookie(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        public string GetCookieName(string accountKey)
        {
            return string.Format(COOKIE_TEMPLATE, accountKey);
        }

        public string GetVisitorId(string accountKey)
        {            
            var cookieName = GetCookieName(accountKey);

            var visitorId = _httpContext.Request.Cookies[cookieName];

            if (visitorId == null || string.IsNullOrWhiteSpace(visitorId.Value))
            {
                return null;
            }

            return visitorId.Value;
        }  

        public string GenerateVisitorId()
        {
            return Guid.NewGuid().ToString();
        }

        public string GetOrCreateVisitorId(string accountKey)
        {
            var visitorId = GetVisitorId(accountKey);

            if (!string.IsNullOrWhiteSpace(visitorId))
            {
                return visitorId;
            }
            else
            {
                return GenerateVisitorId();
            }
        }
    }
}