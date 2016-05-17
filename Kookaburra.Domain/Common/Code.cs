using System.Text;

namespace Kookaburra.Domain.Common
{
    public class Code
    {
        public string GenerateCode(string host, string accountId)
        {
            var code = new StringBuilder();

            code.Append("<!--start of OnlinChat js code-->");
            code.Append("<script type='text/javascript'>");
            code.Append("(function ()");
            code.Append("{");
            code.Append("var oc = document.createElement('script');");
            code.Append("oc.type = 'text/javascript';");
            code.Append("oc.async = true;");
            code.AppendFormat("oc.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + '{0}/{1}';", host, accountId);
            code.Append("var s = document.getElementsByTagName('script')[0];");
            code.Append(" s.parentNode.insertBefore(oc, s);");
            code.Append("}());</script><!--end of OnlineChat js code-->");

            return code.ToString();
        }
    }
}