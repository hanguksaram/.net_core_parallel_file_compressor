using System;
using System.IO;

namespace Gzip.Test.Core
{
    public class StreamHandler<T> : IDisposable where T: Stream
    {
        private readonly T _targetStream;
        private readonly T _sourceStream;
        private readonly IStreamProducerConsumerHandler<T> _streamHandler;
        internal void ExecuteOperation() => _streamHandler.Handle(_targetStream, _sourceStream);
        internal StreamHandler(IStreamProducerConsumerHandler<T> handler, T targetStream, T sourceStream)
        {
            _streamHandler = handler;
            _targetStream = targetStream;
            _sourceStream = sourceStream;
        }
        public void Dispose()
        {
            _targetStream?.Dispose();
            _sourceStream?.Dispose();
        }
    }
}