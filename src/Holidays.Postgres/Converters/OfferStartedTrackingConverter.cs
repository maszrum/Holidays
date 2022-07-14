using Holidays.Core.OfferModel;
using Holidays.Postgres.DbRecords;

namespace Holidays.Postgres.Converters;

internal class OfferStartedTrackingConverter : EventConverterBase<OfferStartedTracking>
{
    protected override Guid GetOfferId(OfferStartedTracking @event) => @event.OfferId;

    protected override string GetEventParams(OfferStartedTracking @event) => string.Empty;

    protected override OfferStartedTracking ToObject(OfferEventLogRecord record) => 
        new(default, record.OfferId, record.EventTimestamp);
}
