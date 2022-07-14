using Holidays.Core.Eventing;

namespace Holidays.Eventing;

public interface IEventBus : IAsyncDisposable
{
    Task Publish(
        IEvent @event,
        CancellationToken cancellationToken = default);
}
