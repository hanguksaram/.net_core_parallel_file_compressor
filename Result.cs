using System;

namespace Gzip.Test
{
    internal class Result<T> : Result
    {
        //not nullable
        private T Value { get; }
        private bool IsSuccess { get; }
        private Exception? Error { get; }

        public Result(T value)
        {
            Value = value;
            IsSuccess = true;
        }

        public Result(Exception? error)
        {
            Value = default;
            IsSuccess = false;
            Error = error;
        }

        internal Result<T> OnError(Action<Exception> fn)
        {
            if (Error != null)
            {
                fn(Error);
            }

            return this;
        }
        
        internal Result<T> OnSuccess(Action<T> fn)
        {
            if (Error == null && Value != null)
            {
                fn(Value);
            }

            return this;
        }

        internal H OnComplete<H>(Func<T, H> fn)
        {
            if (Error == null && Value != null)
            {
                return fn(Value);
            }
            
            return default;
        }
    }

    internal class Result
    {
        internal static Result<T> Of<T>(T value) => new Result<T>(value);
        internal static Result<T> Error<T>(Exception ex) => new Result<T>(ex);
    }
}