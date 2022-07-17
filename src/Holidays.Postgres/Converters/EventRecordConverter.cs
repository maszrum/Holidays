using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;
using Holidays.Postgres.DbRecords;

namespace Holidays.Postgres.Converters;

internal class EventRecordConverter
{
    private static readonly IReadOnlyDictionary<string, IEventConverter> Converters =
        new Dictionary<string, IEventConverter>
        {
            [nameof(OfferAdded)] = new OfferAddedConverter(),
            [nameof(OfferRemoved)] = new OfferRemovedConverter(),
            [nameof(OfferPriceChanged)] = new OfferPriceChangedConverter(),
            [nameof(OfferStartedTracking)] = new OfferStartedTrackingConverter()
        };

    public OfferEventLogRecord ConvertToRecord(IEvent @event)
    {
        var converterKey = @event.GetType().Name;
        var converter = GetConverter(converterKey);

        if (!converter.TryConvertToRecord(@event, out var record))
        {
            throw new InvalidOperationException(
                "Cannot convert from event to record: invalid converter implementation.");
        }

        return record;
    }

    public IEvent ConvertToObject(OfferEventLogRecord record)
    {
        var converterKey = record.EventType;
        var converter = GetConverter(converterKey);

        var @event = converter.ConvertToObject(record);
        return @event;
    }

    private IEventConverter GetConverter(string key)
    {
        if (!Converters.TryGetValue(key, out var converter))
        {
            throw new InvalidOperationException(
                "Cannot convert from event to record: unknown converter.");
        }

        return converter;
    }
}
