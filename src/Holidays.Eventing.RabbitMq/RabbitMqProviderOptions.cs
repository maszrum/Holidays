using System.Reflection;
using Holidays.Core.Eventing;

namespace Holidays.Eventing.RabbitMq;

public class RabbitMqProviderOptions
{
    public HashSet<Assembly> EventTypesAssemblies { get; } = new();
    
    public Func<string, Task>? UnknownEventTypeAction { get; private set; }
    
    public Func<Exception, string, Task>? EventDeserializationError { get; private set; }

    public void AddEventTypesAssembly<TEvent>() 
        where TEvent : IEvent
    {
        var assembly = typeof(TEvent).Assembly;
        EventTypesAssemblies.Add(assembly);
    }

    public void OnUnknownEventType(Func<string, Task> action)
    {
        UnknownEventTypeAction = action;
    }

    public void OnDeserializationError(Func<Exception, string, Task> action)
    {
        EventDeserializationError = action;
    }
}
