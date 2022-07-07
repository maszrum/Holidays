using Holidays.Core.Eventing;

namespace Holidays.Eventing.Tests;

internal class TestEventSecondHandler : IEventHandler<TestEvent>
{
    private readonly Action<TestEvent> _onHandle;

    public TestEventSecondHandler(Action<TestEvent> onHandle)
    {
        _onHandle = onHandle;
    }

    public Task Handle(TestEvent @event, CancellationToken cancellationToken)
    {
        _onHandle(@event);
        return Task.CompletedTask;
    }
}
