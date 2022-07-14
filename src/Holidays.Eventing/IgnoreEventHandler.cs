using Holidays.Core.Eventing;

namespace Holidays.Eventing;

internal class IgnoreEventHandler : IEventHandler<IEvent>
{
    public Task Handle(IEvent @event, Func<Task> next, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
