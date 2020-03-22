using System.IO;
using Gzip.Test.Core;
using static Gzip.Test.PlatformDependentDataConfiguration;

namespace Gzip.Test.Impl
{
    internal sealed class ParallelCompressionHandler : IStreamProducerConsumerHandler<Stream>
    {
        private readonly ICompressor _compressor;

        public ParallelCompressionHandler(ICompressor compressor)
        {
            _compressor = compressor;
        }

        //просто хэндлинг без диспоза стримов
        public Stream Handle(Stream targetStream, Stream sourceStream)
        {
            var sourceStreamLength = sourceStream.Length;
            float chunkF = (float) sourceStream.Length / ChunkFileSize;
            int chunkI = (int) sourceStream.Length / ChunkFileSize;
            var bufferRead = new byte[ChunkFileSize];
            float toComp = chunkI;
            var chunkTotalCount = toComp < chunkF
                ? sourceStreamLength / ChunkFileSize + 1
                : sourceStreamLength / ChunkFileSize;

            var chunksCounter = 0L;
            var activeThreadsCounter = 0;
            int readedLength;
            var bufferAccumulator = new byte[CoresCount][];

            while (0 != (readedLength = sourceStream.Read(bufferRead, 0, ChunkFileSize)))
            {
                var threadIndex = activeThreadsCounter;
                var length = readedLength;
                var read = bufferRead;

                ThreadPool.SyncedTask(() =>
                {
                    bufferAccumulator = CompressThreadSafely(bufferAccumulator, read, length, threadIndex);
                });

                activeThreadsCounter++;
                chunksCounter++;

                //блочим по кол-ву ядер
                if (activeThreadsCounter == CoresCount || chunkTotalCount == chunksCounter)
                {
                    ThreadPool.AwaitAll();

                    for (int i = 0; i < activeThreadsCounter; i++)
                    {
                        WriteCompressedChunkToFile(targetStream, bufferAccumulator[i]);
                    }

                    bufferAccumulator = new byte[CoresCount][];

                    activeThreadsCounter = 0;
                }

                bufferRead = new byte[ChunkFileSize];
            }

            return targetStream;
        }

        private Stream WriteCompressedChunkToFile(Stream targetStream, byte[] compressedChunk)
        {
            var lengthToStore = Utils.GetBytesToStore(compressedChunk.Length);
            targetStream.Write(lengthToStore, 0, lengthToStore.Length);
            targetStream.Write(compressedChunk, 0, compressedChunk.Length);
            return targetStream;
        }

        private byte[][] CompressThreadSafely(byte[][] bufferAccumulator, byte[] readBuffer,
            int readedLength, int threadIndex)
        {
            bufferAccumulator[threadIndex] = _compressor.Compress(readBuffer, readedLength);
            return bufferAccumulator;
        }
    }
}