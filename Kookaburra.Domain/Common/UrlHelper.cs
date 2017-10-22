namespace Kookaburra.Domain.Common
{
    public static class UrlHelper
    {
        public static string OfflineMessageUrl(string host, long id)
        {
            return $"{host}/messages/{id}";
        }
    }
}