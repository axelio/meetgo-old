using System;

namespace MeetAndGo.Infrastructure.Utils
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, string error)
        {
            switch (isSuccess)
            {
                case true when error != string.Empty:
                    throw new InvalidOperationException();
                case false when error == string.Empty:
                    throw new InvalidOperationException();
                default:
                    IsSuccess = isSuccess;
                    Error = error;
                    break;
            }
        }

        public static Result Fail(string message)
        {
            return new(false, message);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new(default, false, message);
        }

        public static Result Ok()
        {
            return new(true, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new(value, true, string.Empty);
        }

        public static Result Combine(params Result[] results)
        {
            foreach (var result in results)
            {
                if (result.IsFailure)
                    return result;
            }

            return Ok();
        }
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        public T Value
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException();

                return _value;
            }
        }

        protected internal Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            _value = value;
        }
    }
}
