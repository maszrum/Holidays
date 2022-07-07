namespace Holidays.Selenium;

public static class WithTimeout
{
    public static async Task<T> Do<T>(TimeSpan timeout, Func<CancellationToken, Task<T>> func)
    {
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(timeout);

        T? result = default;
        try
        {
            result = await func(cts.Token);
        }
        catch (OperationCanceledException)
        {
            if (!cts.IsCancellationRequested)
            {
                throw;
            }
        }

        if (cts.Token.IsCancellationRequested)
        {
            throw new TimeoutException();
        }

        return result!;
    }
}
