using System;

namespace Gzip.Test
{
    internal sealed class PlatformDependentDataConfiguration
    {
        internal const string AppName = "GzipTest";
        internal const int ChunkFileSize = 1048576;//1mb chunk
        internal static readonly int CoresCount = Environment.ProcessorCount;
    }
}