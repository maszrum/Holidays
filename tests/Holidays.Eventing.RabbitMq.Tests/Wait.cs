namespace Holidays.Eventing.RabbitMq.Tests;

internal static class Wait
{
    public static Task<bool> Until(Func<bool> func, TimeSpan timeout)
    {
        return func() 
            ? Task.FromResult(true) 
            : EnterCheckLoop();

        async Task<bool> EnterCheckLoop()
        {
            using var cts = new CancellationTokenSource(timeout);

            while (!cts.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(30, cts.Token);
                }
                catch (TaskCanceledException)
                {
                    return func();
                }

                if (func())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
