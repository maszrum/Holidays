using Holidays.Core.Eventing;

namespace Holidays.Eventing;

public interface IEventBus : IAsyncDisposable
{
    bool RequiresInitialization { get; }

    Task<IEventBus> Initialize();

    Task Publish(
        IEvent @event,
        CancellationToken cancellationToken = default);
}
