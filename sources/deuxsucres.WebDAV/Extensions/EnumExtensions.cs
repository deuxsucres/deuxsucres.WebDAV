using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Extensions method
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Return the header value for a depth value
        /// </summary>
        public static string ToHeaderValue(this DepthValue depth)
        {
            switch (depth)
            {
                case DepthValue.Zero:
                    return "0";
                case DepthValue.One:
                    return "1";
                case DepthValue.Infinity:
                default:
                    return "infinity";
            }
        }

    }
}
