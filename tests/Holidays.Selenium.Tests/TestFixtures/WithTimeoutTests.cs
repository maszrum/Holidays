using System.Diagnostics;
using NUnit.Framework;

namespace Holidays.Selenium.Tests.TestFixtures;

[TestFixture]
public class WithTimeoutTests
{
    [Test]
    public void too_long_task_should_throw_timeout_exception()
    {
        Assert.ThrowsAsync<TimeoutException>(() =>
        {
            return WithTimeout.Do(
                TimeSpan.FromSeconds(1),
                async cancellationToken =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
                    return 10;
                });
        });
    }

    [Test]
    public async Task cancellation_token_should_be_cancelled_when_timed_out()
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await WithTimeout.Do(
                TimeSpan.FromSeconds(1),
                async cancellationToken =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                    return 10;
                });
        }
        catch (TimeoutException)
        {
            // ignored
        }

        stopwatch.Stop();

        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(2000));
    }

    [Test]
    public async Task task_should_not_timeout_and_result_value_should_be_correct()
    {
        var result = await WithTimeout.Do(
            TimeSpan.FromSeconds(3),
            async cancellationToken =>
            {
                await Task.Delay(1, cancellationToken);
                return 33;
            });

        Assert.That(result, Is.EqualTo(33));
    }

    [Test]
    public void throw_operation_cancelled_exception_inside_task_and_it_should_not_throw_timeout_exception()
    {
        Assert.ThrowsAsync<OperationCanceledException>(() =>
        {
            return WithTimeout.Do<int>(
                TimeSpan.FromSeconds(2),
                async cancellationToken =>
                {
                    await Task.Delay(10, cancellationToken);
                    throw new OperationCanceledException();
                });
        });
    }
}
