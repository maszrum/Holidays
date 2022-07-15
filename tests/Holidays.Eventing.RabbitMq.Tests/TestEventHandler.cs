using Holidays.Core.Eventing;

namespace Holidays.Eventing.RabbitMq.Tests;

internal class TestEventHandler<TEvent> : IEventHandler<TEvent>
    where TEvent : IEvent
{
    private readonly List<Action<TEvent>> _onHandle = new();

    public bool Committed { get; private set; }

    public bool RolledBack { get; private set; }

    public async Task Handle(TEvent @event, Func<Task> next, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var onHandle in _onHandle)
            {
                onHandle(@event);
            }

            await next();

            Committed = true;
        }
        catch
        {
            RolledBack = true;
            throw;
        }
    }

    public TestEventHandler<TEvent> DoOnEvent(Action<TEvent> onEvent)
    {
        _onHandle.Add(onEvent);
        return this;
    }
}
