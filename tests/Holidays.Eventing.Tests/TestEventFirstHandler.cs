using Holidays.Core.Eventing;

namespace Holidays.Eventing.Tests;

internal class TestEventFirstHandler : IEventHandler<TestEvent>
{
    private readonly Action<TestEvent> _onHandle;

    public TestEventFirstHandler(Action<TestEvent> onHandle)
    {
        _onHandle = onHandle;
    }

    public Task Handle(TestEvent @event, CancellationToken cancellationToken)
    {
        _onHandle(@event);
        return Task.CompletedTask;
    }
}
