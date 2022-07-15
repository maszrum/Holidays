namespace Holidays.Core.Monads;

public class Result<TData> where TData : notnull
{
    private readonly TData? _data;
    private readonly ErrorBase? _error;

    private Result(TData data)
    {
        _data = data;
        _error = default;
    }

    private Result(ErrorBase error)
    {
        _data = default;
        _error = error;
    }

    public TData Data => _data ?? throw new InvalidOperationException("Result is error.");

    public ErrorBase Error => _error ?? throw new InvalidOperationException("Result is data.");

    public bool IsError => _error is not null;

    public T Match<T>(Func<ErrorBase, T> onError, Func<TData, T> onData)
    {
        return _data is null
            ? onError(_error!)
            : onData(_data);
    }

    public Result<TResult> Then<TResult>(Func<TData, Result<TResult>> func)
        where TResult : notnull
    {
        return _data is null
            ? new Result<TResult>(_error!)
            : func(_data);
    }

    public static implicit operator Result<TData>(TData data) => new(data);

    public static implicit operator Result<TData>(ErrorBase error) => new(error);
}

public static class Result
{
    public static Result<TData> Success<TData>(TData data)
        where TData : notnull
    {
        return data;
    }

    public static Result<TData> Error<TData>(ErrorBase error)
        where TData : notnull
    {
        return error;
    }
}
