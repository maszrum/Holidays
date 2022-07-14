namespace Holidays.Eventing;

public class EventHandlerDescriptor
{
    public EventHandlerDescriptor(Func<object> handlerFactory, bool onlyForLocalEvents)
    {
        HandlerFactory = handlerFactory;
        OnlyForLocalEvents = onlyForLocalEvents;
    }

    public Func<object> HandlerFactory { get; }
    
    public bool OnlyForLocalEvents { get; }
}
