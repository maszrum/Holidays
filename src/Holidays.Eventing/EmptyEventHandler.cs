using Holidays.Eventing.Core;

namespace Holidays.Eventing;

internal class EmptyEventHandler : IEventHandler<IEvent>
{
    public Task Handle(IEvent @event, Func<Task> next, CancellationToken cancellationToken)
    {
        return next();
    }
}
