using System.Collections.Immutable;
using System.Text;
using Holidays.Core.Eventing;
using Newtonsoft.Json;

namespace Holidays.Eventing.RabbitMq;

internal class EventConverter
{
    private readonly ImmutableDictionary<string, Type> _eventTypes;
    
    public EventConverter(IEnumerable<Type> eventTypes)
    {
        _eventTypes = eventTypes.ToImmutableDictionary(
            t => t.Name, 
            t => t);
    }

    public bool IsTypeRegistered(string typeName) => _eventTypes.ContainsKey(typeName);
    
    public IEvent ConvertToEvent(string typeName, ReadOnlySpan<byte> bytes)
    {
        if (!_eventTypes.TryGetValue(typeName, out var eventType))
        {
            throw new InvalidOperationException(
                $"Cannot convert from json to event type {typeName}: cannot find specified type.");
        }

        var json = Encoding.UTF8.GetString(bytes);
        
        var @event = JsonConvert.DeserializeObject(json, eventType);

        if (@event is not IEvent eventTyped)
        {
            throw new InvalidOperationException(
                $"Cannot convert from json to event type {typeName}.");
        }

        return eventTyped;
    }

    public ReadOnlyMemory<byte> ConvertToBytes(IEvent @event)
    {
        var json = JsonConvert.SerializeObject(@event);

        var bytes = Encoding.UTF8.GetBytes(json);

        return bytes;
    }
}
