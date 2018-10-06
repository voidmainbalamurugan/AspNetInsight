using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetInsight4
{
    /// <summary>
    /// Memory stream wrapper used to get its size at any point in time.
    /// </summary>
    public class ResponseStreamWrapper : MemoryStream
    {
        private readonly Stream _responseStream;
        public long Size { get; protected set; }
        public ResponseStreamWrapper(Stream outputStream)
        {
            _responseStream = outputStream;
            Size = 0;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Size += count;
            _responseStream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            _responseStream.Flush();
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            Size += count;
            return _responseStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void WriteByte(byte value)
        {
            Size += 1;
            _responseStream.WriteByte(value);
        }
        
    }
}
