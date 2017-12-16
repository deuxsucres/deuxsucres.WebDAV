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
        protected abstract ContentLineParameter InternalSerialize(ContentSyntax syntax);

        /// <summary>
        /// Serialize the parameter for a content line
        /// </summary>
        public ContentLineParameter Serialize(ContentSyntax syntax)
        {
            return InternalSerialize(syntax ?? throw new ArgumentNullException(nameof(syntax)));
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
            return InternalDeserialize(
                param ?? throw new ArgumentNullException(nameof(param)),
                syntax ?? throw new ArgumentNullException(nameof(syntax))
                );
        }
        #endregion

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

    }
}
