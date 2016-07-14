using System;

namespace Kookaburra.Common
{
    public static class ExtensionMethods
    {
        public static double JsDateTime(this DateTime dateTime)
        {
            return dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}