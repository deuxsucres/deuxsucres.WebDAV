using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// List of text content parameter
    /// </summary>
    public class TextListContentParameter : ContentParameter
    {
        /// <summary>
        /// Deserialize
        /// </summary>
        protected override bool InternalDeserialize(ContentLineParameter param, ContentSyntax syntax)
        {
            Value = new List<string>(param.Values);
            return true;
        }

        /// <summary>
        /// Serialize
        /// </summary>
        protected override void InternalSerialize(ContentLineParameter parameter, ContentSyntax syntax)
        {
            parameter.Values.Clear();
            if (Value != null)
                parameter.Values.AddRange(Value);
        }

        /// <summary>
        /// Cast to a string array
        /// </summary>
        public static implicit operator string[](TextListContentParameter prop) { return prop?.Value?.ToArray(); }

        /// <summary>
        /// Cast from a string array
        /// </summary>
        public static implicit operator TextListContentParameter(string[] value) { return value != null ? new TextListContentParameter { Value = new List<string>(value ?? Enumerable.Empty<string>()) } : null; }

        /// <summary>
        /// Cast to a string list
        /// </summary>
        public static implicit operator List<string> (TextListContentParameter prop) { return prop?.Value?.ToList(); }

        /// <summary>
        /// Cast from a string list
        /// </summary>
        public static implicit operator TextListContentParameter(List<string> value) { return value != null ? new TextListContentParameter { Value = new List<string>(value ?? Enumerable.Empty<string>()) } : null; }

        /// <summary>
        /// Value
        /// </summary>
        public List<string> Value { get; set; }
    }
}
