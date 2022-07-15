using Holidays.Core.Eventing;

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

    public Task<IEventBus> Build()
    {
        if (_externalProviders.Count == 0)
        {
            var eventBus = new EventBus(_handlerFactories, _externalProviders);
            return Task.FromResult((IEventBus) eventBus);
        }

        return InitializeAndBuild();

        async Task<IEventBus> InitializeAndBuild()
        {
            var eventBus = new EventBus(_handlerFactories, _externalProviders);

            foreach (var externalProvider in _externalProviders)
            {
                await externalProvider.Initialize(eventBus);
            }

            return eventBus;
        }
    }
}
