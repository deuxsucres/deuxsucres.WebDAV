using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Content writer
    /// </summary>
    public class ContentWriter : IDisposable
    {
        private readonly bool _leaveOpen;
        private bool _disposedValue = false;

        /// <summary>
        /// Create a new writer
        /// </summary>
        public ContentWriter(TextWriter source, bool leaveOpen = false)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            _leaveOpen = leaveOpen;
        }

        #region IDisposable Support

        /// <summary>
        /// Internal dispose
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!_disposedValue && disposing && !_leaveOpen)
                {
                    Source.Dispose();
                }
            }
            finally
            {
                if (!_disposedValue && !_leaveOpen)
                {
                    Source = null;
                }
                _disposedValue = true;
            }
        }

        /// <summary>
        /// Dispose ressources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// Write source
        /// </summary>
        public TextWriter Source { get; private set; }

    }
}