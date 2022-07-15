namespace Holidays.Core.Monads;

public abstract class ErrorBase
{
    public abstract string Message { get; }
}

public abstract class ErrorWithException : ErrorBase
{
    public virtual Exception GetException()
    {
        throw new Exception();
    }
}

public abstract class ErrorBase<TException> : ErrorWithException
    where TException : Exception
{
    public abstract TException Exception { get; }

    public override Exception GetException() => Exception;
}
