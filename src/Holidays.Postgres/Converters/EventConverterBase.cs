using System.Diagnostics.CodeAnalysis;
using Holidays.Core.Eventing;
using Holidays.Postgres.DbRecords;

namespace Holidays.Postgres.Converters;

internal abstract class EventConverterBase<TEvent> : IEventConverter
    where TEvent : IEvent
{
    public bool TryConvertToRecord(IEvent @event, [NotNullWhen(true)] out OfferEventLogRecord? record)
    {
        if (@event is TEvent eventTyped)
        {
            record = ConvertToRecord(eventTyped);
            return true;
        }

        record = default;
        return false;
    }

    public IEvent ConvertToObject(OfferEventLogRecord record) => ToObject(record);

    protected abstract Guid GetOfferId(TEvent @event);

    protected abstract string GetEventParams(TEvent @event);

    protected abstract TEvent ToObject(OfferEventLogRecord record);
    
    private OfferEventLogRecord ConvertToRecord(TEvent @event)
    {
        return new OfferEventLogRecord(
            Guid.NewGuid(),
            @event.Timestamp,
            GetOfferId(@event),
            @event.GetType().Name,
            GetEventParams(@event));
    }
}
