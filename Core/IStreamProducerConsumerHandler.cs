using System.IO;

namespace Gzip.Test.Core
{
    public interface IStreamProducerConsumerHandler<T> where T : Stream
    {
        T Handle(T targetStream, T sourceStream);
    }
}