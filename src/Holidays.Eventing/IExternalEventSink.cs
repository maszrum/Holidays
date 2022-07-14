using Holidays.Core.Eventing;

namespace Holidays.Eventing;

public interface IExternalEventSink
{
    Task Publish(IEvent @event, CancellationToken cancellationToken = default);
}
