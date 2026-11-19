namespace Benchmarks;

public sealed record Error(string Code, string Message);

public readonly struct Result<T>
{
    private readonly T _value;

    private Result(T value, Error? error)
    {
        _value = value;
        Error = error;
    }

    public Error? Error { get; }
    public bool IsSuccess => Error is null;
    public T Value => IsSuccess ? _value : throw new InvalidOperationException("No value on a failed result.");

    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(Error error) => new(default!, error);

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);
}

public sealed class ValidationException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}
