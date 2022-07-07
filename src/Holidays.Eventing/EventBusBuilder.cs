using Holidays.Core.Eventing;

namespace Holidays.Eventing;

public class EventBusBuilder
{
    private readonly Dictionary<Type, List<Func<object>>> _handlerFactories = new();

    public EventBusBuilderForEventType<TEvent> ForEventType<TEvent>() 
        where TEvent : IEvent
    {
        if (_handlerFactories.ContainsKey(typeof(TEvent)))
        {
            throw new InvalidOperationException(
                "Specified event has been registered already.");
        }

        var factories = new List<Func<object>>();
        _handlerFactories.Add(typeof(TEvent), factories);
        
        return new EventBusBuilderForEventType<TEvent>(
            handlerType => factories.Add(handlerType));
    }
    
    public EventBus Build() => new(_handlerFactories);
}
