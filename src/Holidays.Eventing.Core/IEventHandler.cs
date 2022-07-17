namespace Holidays.Eventing.Core;

public interface IEventHandler<in TEvent>
    where TEvent : IEvent
{
    Task Handle(TEvent @event, Func<Task> next, CancellationToken cancellationToken);
}
