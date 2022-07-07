using System.Diagnostics.CodeAnalysis;

namespace Holidays.Core.Monads;

public class Maybe<TData> where TData : notnull
{
    private readonly TData? _data;

    private Maybe()
    {
        _data = default;
    }

    private Maybe(TData data)
    {
        _data = data;
    }

    public TData Data => _data ?? throw new InvalidOperationException("Data is null");

    public bool IsNull => _data is not null;

    public bool TryGetData([NotNullWhen(true)] out TData? data)
    {
        if (_data is null)
        {
            data = default;
            return false;
        }

        data = _data;
        return true;
    }
    
    public T Match<T>(Func<T> onNull, Func<TData, T> onData)
    {
        return _data is null
            ? onNull()
            : onData(_data);
    }
    
    public void IfNotNull(Action<TData> onData)
    {
        if (_data is not null)
        {
            onData(_data);
        }
    }

    public static implicit operator Maybe<TData>(TData? data)
    {
        return data is null 
            ? new Maybe<TData>() 
            : new Maybe<TData>(data);
    }
}

public static class Maybe
{
    public static Maybe<TData> Null<TData>() where TData : notnull
    {
        return default(TData);
    }

    public static Maybe<TData> Data<TData>(TData data) where TData : notnull
    {
        return data;
    }
}
