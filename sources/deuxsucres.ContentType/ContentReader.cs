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
        /// Check if the reader is disposed
        /// </summary>
        protected void CheckNotDisposed()
        {
            if (_disposedValue)
                throw new ObjectDisposedException(GetType().Name);
        }

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
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// Read the next unfolded line of text
        /// </summary>
        public virtual string ReadLine()
        {
            CheckNotDisposed();

            // Read the next line
            string line = Source.ReadLine();
            if (line == null)
                return null;

            // Unfold next lines
            int peek;
            while (line == string.Empty || (peek = Source.Peek()) == 9 || peek == 32)
            {
                string sline = Source.ReadLine();
                if (line != string.Empty)
                    line += sline.Substring(1);
                else
                    line = sline;
            }

            return line;
        }

        /// <summary>
        /// Read source
        /// </summary>
        public TextReader Source { get; private set; }

    }
}
