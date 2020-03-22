namespace Gzip.Test.Core
{
    internal interface IDecompressor
    {
        byte[] Decompress(byte[] compressedBytes);
    }
}