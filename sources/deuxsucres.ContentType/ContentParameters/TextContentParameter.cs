using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Text content parameter
    /// </summary>
    public class TextContentParameter : ContentParameter
    {
        /// <summary>
        /// Deserialize
        /// </summary>
        protected override bool InternalDeserialize(ContentLineParameter param, ContentSyntax syntax)
        {
            Value = param.Value;
            return true;
        }

        /// <summary>
        /// Serialize
        /// </summary>
        protected override void InternalSerialize(ContentLineParameter parameter, ContentSyntax syntax)
        {
            parameter.Value = Value;
        }

        /// <summary>
        /// Cast to a string
        /// </summary>
        public static implicit operator string(TextContentParameter prop) { return prop?.Value; }

        /// <summary>
        /// Cast from a string
        /// </summary>
        public static implicit operator TextContentParameter(string value) { return value != null ? new TextContentParameter { Value = value } : null; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }
    }
}
