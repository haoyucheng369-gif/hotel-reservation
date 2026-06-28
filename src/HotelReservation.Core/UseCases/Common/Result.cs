namespace HotelReservation.Core.UseCases.Common;

public sealed class Result
{
    private Result(string? error, bool isSuccess)
    {
        Error = error;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }

    public string? Error { get; }

    public static Result Success()
    {
        return new Result(null, true);
    }

    public static Result Failure(string error)
    {
        return new Result(error, false);
    }
}

public sealed class Result<T>
{
    private Result(T? value, string? error, bool isSuccess)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; }

    public T? Value { get; }

    public string? Error { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, null, true);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(default, error, false);
    }
}
