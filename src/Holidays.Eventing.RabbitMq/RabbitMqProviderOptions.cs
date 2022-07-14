using Holidays.Core.Eventing;

namespace Holidays.Eventing.RabbitMq;

public class RabbitMqProviderOptions
{
    public Action<string>? UnknownEventTypeAction { get; private set; }
    
    public Action<Exception, string>? EventDeserializationError { get; private set; }
    
    public Action<IEvent>? EventReceivedLogAction { get; private set; }
    
    public Action<IEvent>? EventSentLogAction { get; private set; }
    
    public void OnUnknownEventType(Action<string> action)
    {
        UnknownEventTypeAction = action;
    }

    public void OnDeserializationError(Action<Exception, string> action)
    {
        EventDeserializationError = action;
    }

    public void OnEventReceived(Action<IEvent> action)
    {
        EventReceivedLogAction = action;
    }

    public void OnEventSent(Action<IEvent> action)
    {
        EventSentLogAction = action;
    }
}
