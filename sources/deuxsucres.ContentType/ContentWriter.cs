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
        /// Check if the writer is disposed
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
        /// Write a line and fold it if the length is greater then <see cref="LineSize"/>
        /// </summary>
        /// <param name="line">Line of text to write</param>
        /// <returns>Count of real lines written</returns>
        public virtual int WriteLine(string line)
        {
            CheckNotDisposed();
            if (string.IsNullOrEmpty(line)) return 0;
            string prefix = "";
            int count = 0;
            // If the line size is <=1 then no folding is required
            if (LineSize <= 1)
            {
                Source.WriteLine(line);
                count = 1;
            }
            else
            {
                int lineSize = LineSize;
                while (true)
                {
                    if (line.Length > lineSize)
                    {
                        string tLine = line.Substring(0, lineSize);
                        line = line.Substring(lineSize);
                        Source.WriteLine(prefix + tLine);
                        prefix = " ";
                        lineSize = LineSize - 1;
                        count++;
                    }
                    else
                    {
                        Source.WriteLine(prefix + line);
                        count++;
                        break;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Write source
        /// </summary>
        public TextWriter Source { get; private set; }

        /// <summary>
        /// Max line size
        /// </summary>
        /// <remarks>
        /// If a line is greater than the line is folded.
        /// If <see cref="LineSize"/> is &lt;=1 then no folding is required
        /// </remarks>
        public int LineSize { get; set; } = 75;

    }
}