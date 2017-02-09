using System.Linq;

namespace Kookaburra.Domain.Common
{
    public static class Helper
    {
        public static bool ValidateIPv4(string ipString)
        {
            if (string.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }
    }
}