using System;
using System.Collections.Generic;
using System.Linq;

namespace Kookaburra.Common
{
    public static class ExtensionMethods
    {
        public static double JsDateTime(this DateTime dateTime)
        {
            return dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static List<string> AllBut(this List<string> originalList, string excluded)
        {
            if (originalList == null) return null;

            return originalList.Where(l => l != excluded).ToList();
        }
    }
}