using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Content parameter
    /// </summary>
    public abstract class ContentParameter
    {

        #region Serialization
        /// <summary>
        /// Internal serialization
        /// </summary>
        protected abstract void InternalSerialize(ContentLineParameter parameter, ContentSyntax syntax);

        /// <summary>
        /// Serialize the parameter for a content line
        /// </summary>
        public ContentLineParameter Serialize(ContentSyntax syntax)
        {
            if (syntax == null) throw new ArgumentNullException(nameof(syntax));
            if (string.IsNullOrWhiteSpace(Name)) return null;
            ContentLineParameter result = new ContentLineParameter(Name);
            InternalSerialize(result, syntax);
            return result;
        }

        /// <summary>
        /// Internal deserialization
        /// </summary>
        protected abstract bool InternalDeserialize(ContentLineParameter param, ContentSyntax syntax);

        /// <summary>
        /// Deserialize the parameter from a content line
        /// </summary>
        public bool Deserialize(ContentLineParameter param, ContentSyntax syntax)
        {
            if (InternalDeserialize(
                param ?? throw new ArgumentNullException(nameof(param)),
                syntax ?? throw new ArgumentNullException(nameof(syntax))
                ))
            {
                Name = (param).Name;
                return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

    }
}
