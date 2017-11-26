using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// COMMENT property
    /// </summary>
    public class CommentProperty : ExtendedTextProperty
    {

        /// <summary>
        /// New property
        /// </summary>
        public CommentProperty()
        {
            Name = Constants.COMMENT;
        }

        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator string(CommentProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator CommentProperty(string value) { return value != null ? new CommentProperty { Value = value } : null; }

    }
}
