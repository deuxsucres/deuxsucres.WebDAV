using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Check if two string are equals without case sensitive
        /// </summary>
        public static bool IsEqual(this string from, string other)
        {
            return string.Equals(from, other, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
