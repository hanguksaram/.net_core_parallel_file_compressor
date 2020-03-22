using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Gzip.Test.Core;
using static Gzip.Test.PlatformDependentDataConfiguration;


namespace Gzip.Test.Impl
{
    internal sealed class ParallelDecompressionHandler : IStreamProducerConsumerHandler<Stream>
    {
        private readonly IDecompressor _decompressor;

        public ParallelDecompressionHandler(IDecompressor gzipDecompressor)
        {
            _decompressor = gzipDecompressor;
        }

        public Stream Handle(Stream targetStream, Stream sourceStream)
        {
            int readLength;
            var activeThreadCounter = 0;
            var chunkCounter = 0L;
            var bufferAccumulator = new byte[CoresCount][];
            var firstBuffer = new byte[8];

            while (0 != (readLength = sourceStream.Read(firstBuffer, 0, 8)))
            {
                if (readLength != 8)
                {
                    throw new Exception(
                        "File Corrupted");
                }

                int lengthToRead = Utils.GetLengthFromBytes(firstBuffer);

                var readBuffer = new byte[lengthToRead];

                if (lengthToRead != sourceStream.Read(readBuffer, 0, lengthToRead))
                {
                    throw new Exception(
                        "File Corrupted");
                }

                var counter = activeThreadCounter;
                var buffer = readBuffer;
                ThreadPool.SyncedTask(() =>
                {
                    bufferAccumulator = DecompressThreadSafely(counter, bufferAccumulator, buffer);
                });

                activeThreadCounter++;
                chunkCounter++;

                if (activeThreadCounter == CoresCount || sourceStream.Position == sourceStream.Length)
                {
                    ThreadPool.AwaitAll();

                    for (var i = 0; i < activeThreadCounter; i++)
                    {
                        WriteDecompressedChunkToFile(targetStream, bufferAccumulator[i]);
                    }

                    bufferAccumulator = new byte[CoresCount][];
                    activeThreadCounter = 0;
                }
            }
            
            return targetStream;
        }

        private Stream WriteDecompressedChunkToFile(Stream targetStream, byte[] decompressedBytes)
        {
            targetStream.Write(decompressedBytes, 0, decompressedBytes.Length);
            return targetStream;
        }

        private byte[][] DecompressThreadSafely(int threadIndex,
            byte[][] bufferAccumulator, byte[] bufferReader)
        {
            bufferAccumulator[threadIndex] = _decompressor.Decompress(bufferReader);
            return bufferAccumulator;
        }
        
    }
}