namespace Gzip.Test.Core
{
    internal interface ICompressor
    {
        byte[] Compress(byte[] bytesToCompress, int readedLength);
    }
}