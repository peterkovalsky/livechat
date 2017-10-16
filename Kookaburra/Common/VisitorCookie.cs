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

        public string GetVisitorKey(string accountKey)
        {            
            var cookieName = GetCookieName(accountKey);

            var visitorKey = _httpContext.Request.Cookies[cookieName];

            if (visitorKey == null || string.IsNullOrWhiteSpace(visitorKey.Value))
            {
                return null;
            }

            return visitorKey.Value;
        }  

        public string GenerateVisitorKey()
        {
            return Guid.NewGuid().ToString();
        }

        public string GetOrCreateVisitorKey(string accountKey)
        {
            var visitorKey = GetVisitorKey(accountKey);

            if (!string.IsNullOrWhiteSpace(visitorKey))
            {
                return visitorKey;
            }
            else
            {
                return GenerateVisitorKey();
            }
        }
    }
}