using System.IO;
using System.IO.Compression;
using Gzip.Test.Core;

namespace Gzip.Test.Impl
{
    internal class GzipCompressor : ICompressor
    {
        public byte[] Compress(byte[] bytesToCompress, int readedLength)
        {
            var stream = new MemoryStream(); 
            var gzStream = new GZipStream(stream, CompressionMode.Compress, true);
            gzStream.Write(bytesToCompress, 0, readedLength);
            gzStream.Close();
            stream.Close();
            return stream.ToArray();
        }
    }
}