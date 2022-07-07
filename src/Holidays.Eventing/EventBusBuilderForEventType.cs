using Holidays.Core.Eventing;

namespace Holidays.Eventing;

public class EventBusBuilderForEventType<TEvent> 
    where TEvent : IEvent
{
    private readonly Action<Func<object>> _onRegister;

    public EventBusBuilderForEventType(Action<Func<object>> onRegister)
    {
        _onRegister = onRegister;
    }

    public EventBusBuilderForEventType<TEvent> RegisterHandler<TEventHandler>(Func<TEventHandler> handlerFactory)
        where TEventHandler : class, IEventHandler<TEvent>
    {
        _onRegister(handlerFactory);
        return this;
    }
}
