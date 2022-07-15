using Holidays.Core.Eventing;

namespace Holidays.Eventing;

public class EventBusBuilderForEventType<TEvent>
    where TEvent : IEvent
{
    private readonly Action<EventHandlerDescriptor> _onRegister;

    public EventBusBuilderForEventType(Action<EventHandlerDescriptor> onRegister)
    {
        _onRegister = onRegister;
    }

    public EventBusBuilderForEventType<TEvent> RegisterHandlerForAllEvents<TEventHandler>(Func<TEventHandler> handlerFactory)
        where TEventHandler : class, IEventHandler<TEvent>
    {
        var descriptor = new EventHandlerDescriptor(handlerFactory, onlyForLocalEvents: false);
        _onRegister(descriptor);

        return this;
    }

    public EventBusBuilderForEventType<TEvent> RegisterHandlerForLocalEvents<TEventHandler>(Func<TEventHandler> handlerFactory)
        where TEventHandler : class, IEventHandler<TEvent>
    {
        var descriptor = new EventHandlerDescriptor(handlerFactory, onlyForLocalEvents: true);
        _onRegister(descriptor);

        return this;
    }

    public void NoLocalHandlers()
    {
        var descriptor = new EventHandlerDescriptor(
            () => new EmptyEventHandler(),
            onlyForLocalEvents: false);

        _onRegister(descriptor);
    }
}
