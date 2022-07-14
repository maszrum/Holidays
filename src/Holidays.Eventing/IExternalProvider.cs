namespace Holidays.Eventing;

public interface IExternalProvider : IAsyncDisposable
{
    IExternalEventSource Source { get; }

    IExternalEventSink Sink { get; }

    Task Initialize(EventBus eventBus);
}
