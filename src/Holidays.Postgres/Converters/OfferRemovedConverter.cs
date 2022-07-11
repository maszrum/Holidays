using Holidays.Core.OfferModel;
using Holidays.Postgres.DbRecords;

namespace Holidays.Postgres.Converters;

internal class OfferRemovedConverter : EventConverterBase<OfferRemoved>
{
    protected override Guid GetOfferId(OfferRemoved @event) => @event.OfferId;

    protected override string GetEventParams(OfferRemoved @event) => string.Empty;
    
    protected override OfferRemoved ToObject(OfferEventLogRecord record) => new(record.OfferId);
}
