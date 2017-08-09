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

        public string GetVisitorId(string accountKey)
        {            
            var cookieName = string.Format(COOKIE_TEMPLATE, accountKey);

            var visitorId = _httpContext.Request.Cookies[cookieName];

            if (visitorId == null || string.IsNullOrWhiteSpace(visitorId.Value))
            {
                return null;
            }

            return visitorId.Value;
        }

        public void SetVisitorId(string accountKey, string newVisitorId)
        {            
            var cookieName = string.Format(COOKIE_TEMPLATE, accountKey);

            var visitorId = _httpContext.Request.Cookies[cookieName];

            if (visitorId != null) // update cookie if it already exists
            {
                visitorId.Value = newVisitorId;
            }
            else // create new cookie
            {
                var cookie = new HttpCookie(cookieName, newVisitorId)
                {
                    Expires = DateTime.Now.AddYears(10)
                };

                _httpContext.Response.Cookies.Set(cookie);
            }            
        }

        public string GenerateVisitorId()
        {
            return Guid.NewGuid().ToString();
        }

        public string GetOrCreateVisitorId(string accountKey)
        {
            var visitorId = GetVisitorId(accountKey);

            if (string.IsNullOrWhiteSpace(visitorId))
            {
                return visitorId;
            }
            else
            {
                visitorId = GenerateVisitorId();
                SetVisitorId(accountKey, visitorId);

                return visitorId;
            }
        }
    }
}