using System.Diagnostics.CodeAnalysis;

namespace Holidays.Core.Monads;

public class Maybe<TData> where TData : notnull
{
    private readonly TData? _data;

    private Maybe()
    {
        _data = default;
        IsNone = true;
    }

    private Maybe(TData data)
    {
        _data = data;
        IsNone = false;
    }

    public TData Data
    {
        get
        {
            if (IsNone)
            {
                throw new InvalidOperationException("Data is none.");
            }

            return _data!;
        }
    }

    public bool IsNone { get; }

    public bool TryGetData([NotNullWhen(true)] out TData? data)
    {
        if (IsNone)
        {
            data = default;
            return false;
        }

        data = _data!;
        return true;
    }

    public T Match<T>(Func<T> onNone, Func<TData, T> onData)
    {
        return IsNone
            ? onNone()
            : onData(_data!);
    }

    public TData WithDefaultValue(TData defaultValue)
    {
        return IsNone
            ? defaultValue
            : _data!;
    }

    public void IfSome(Action<TData> onData)
    {
        if (!IsNone)
        {
            onData(_data!);
        }
    }

    public static Maybe<TData> None() => new();

    public static Maybe<TData> Some(TData data) => new(data);
}

public static class Maybe
{
    public static Maybe<TData> None<TData>() where TData : notnull => Maybe<TData>.None();

    public static Maybe<TData> Some<TData>(TData data) where TData : notnull => Maybe<TData>.Some(data);
}
