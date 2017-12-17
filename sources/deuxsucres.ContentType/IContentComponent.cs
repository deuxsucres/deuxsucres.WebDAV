using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Define a content component
    /// </summary>
    public interface IContentComponent
    {
        /// <summary>
        /// Name of the component
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Entity parent
        /// </summary>
        ContentEntity Parent { get; set; }
    }
}
