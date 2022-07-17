using Holidays.Eventing.Core;

namespace Holidays.Eventing;

public class EventBusBuilder
{
    private readonly List<IExternalProvider> _externalProviders = new();
    private readonly Dictionary<Type, List<EventHandlerDescriptor>> _handlerFactories = new();

    public EventBusBuilderForEventType<TEvent> ForEventType<TEvent>()
        where TEvent : IEvent
    {
        if (_handlerFactories.ContainsKey(typeof(TEvent)))
        {
            throw new InvalidOperationException(
                "Specified event has been registered already.");
        }

        var factories = new List<EventHandlerDescriptor>();
        _handlerFactories.Add(typeof(TEvent), factories);

        return new EventBusBuilderForEventType<TEvent>(
            descriptor => factories.Add(descriptor));
    }

    public EventBusBuilder UseExternalProvider(IExternalProvider provider)
    {
        _externalProviders.Add(provider);

        return this;
    }

    public IEventBus Build() => new EventBus(_handlerFactories, _externalProviders);
}
