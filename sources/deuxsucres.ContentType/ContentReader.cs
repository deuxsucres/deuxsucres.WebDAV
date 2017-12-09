using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace deuxsucres.ContentType
{
    /// <summary>
    /// Content reader
    /// </summary>
    public class ContentReader : IDisposable
    {
        private readonly bool _leaveOpen;
        private bool _disposedValue = false;

        /// <summary>
        /// Create a new reader
        /// </summary>
        public ContentReader(TextReader source, bool leaveOpen = false)
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
        /// Read source
        /// </summary>
        public TextReader Source { get; private set; }

    }
}
