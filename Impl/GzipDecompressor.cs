using System.IO;
using System.IO.Compression;
using Gzip.Test.Core;

namespace Gzip.Test.Impl
{
    public class GzipDecompressor : IDecompressor
    {
        public byte[] Decompress(byte[] compressedBytes)
        {
            var compressedChunk = new MemoryStream(compressedBytes);
            var unCompZip = new GZipStream(compressedChunk, CompressionMode.Decompress, true);
            var unCompressedBuffer = new byte[compressedBytes.Length];
            var unCompressedChunk = new MemoryStream();
            var read = 0;

            while (0 != (read = unCompZip.Read(unCompressedBuffer, 0, compressedBytes.Length)))
            {
                unCompressedChunk.Write(unCompressedBuffer, 0, read);
            }
            
            var result = unCompressedChunk.ToArray();

            unCompZip.Close();
            compressedChunk.Close();
            unCompressedChunk.Close();
            
            return result;
        }
    }
}