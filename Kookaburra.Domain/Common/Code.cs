using System.Text;

namespace Kookaburra.Domain.Common
{
    public class Code
    {
        public string GenerateCode(string host, string accountId)
        {
            var code = new StringBuilder();

            code.AppendLine("<!--start of Kookaburra js code-->");
            code.AppendLine("<script type='text/javascript'>");
            code.AppendLine("(function ()");
            code.AppendLine("{");
            code.AppendLine("var oc = document.createElement('script');");
            code.AppendLine("oc.type = 'text/javascript';");
            code.AppendLine("oc.async = true;");
            code.AppendLine($"oc.src = ('https:' == document.location.protocol ? 'https://' : 'http://') + '{host}/{accountId}';");
            code.AppendLine("var s = document.getElementsByTagName('script')[0];");
            code.AppendLine(" s.parentNode.insertBefore(oc, s);");
            code.AppendLine("}());</script><!--end of Kookaburra js code-->");

            return code.ToString();
        }
    }
}