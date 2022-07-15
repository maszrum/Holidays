using Holidays.Core.Eventing;

namespace Holidays.Eventing.Tests;

internal class TestEventSecondHandler : IEventHandler<TestEvent>
{
    private readonly Action<TestEvent> _onHandle;

    public TestEventSecondHandler(Action<TestEvent> onHandle)
    {
        _onHandle = onHandle;
    }

    public bool Committed { get; private set; }

    public bool RolledBack { get; private set; }

    public async Task Handle(TestEvent @event, Func<Task> next, CancellationToken cancellationToken)
    {
        try
        {
            _onHandle(@event);
            await next();

            Committed = true;
        }
        catch
        {
            RolledBack = true;
            throw;
        }
    }
}
