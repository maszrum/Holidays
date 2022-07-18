using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Holidays.Eventing.Core;

namespace Holidays.Eventing.RabbitMq;

internal class EventConverter
{
    private static readonly JsonSerializerOptions JsonOptions;

    private readonly ImmutableDictionary<string, Type> _eventTypes;

    static EventConverter()
    {
        JsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        JsonOptions.Converters.Add(new DateOnlyJsonConverter());
    }

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

        var @event = JsonSerializer.Deserialize(json, eventType, JsonOptions);

        if (@event is not IEvent eventTyped)
        {
            throw new InvalidOperationException(
                $"Cannot convert from json to event type {typeName}.");
        }

        return eventTyped;
    }

    public ReadOnlyMemory<byte> ConvertToBytes(IEvent @event)
    {
        var json = JsonSerializer.Serialize(@event, @event.GetType(), JsonOptions);

        var bytes = Encoding.UTF8.GetBytes(json);

        return bytes;
    }
}
