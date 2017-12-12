using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// CATEGORIES property
    /// </summary>
    public class CategoriesProperty : TextListProperty
    {

        /// <summary>
        /// New property
        /// </summary>
        public CategoriesProperty()
        {
            Name = Constants.CATEGORIES;
        }

        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator string[] (CategoriesProperty prop) { return prop?.Value?.ToArray(); }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator CategoriesProperty(string[] value) { return value != null ? new CategoriesProperty { Value = value.ToList() } : null; }

        /// <summary>
        /// Language
        /// </summary>
        public TextParameter Language
        {
            get { return FindParameter<TextParameter>(Constants.LANGUAGE); }
            set { SetParameter(value, Constants.LANGUAGE); }
        }
    }
}
