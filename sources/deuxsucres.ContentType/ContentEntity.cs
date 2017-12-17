using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Entity of content
    /// </summary>
    public abstract class ContentEntity : IContentComponent
    {
        #region IContentComponent
        ContentEntity IContentComponent.Parent { get; set; }
        #endregion

        /// <summary>
        /// Name
        /// </summary>
        public abstract string Name { get; }
    }
}
