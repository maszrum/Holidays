using Holidays.Core.OfferModel;

namespace Holidays.MongoDb.Converters;

internal class OfferRemovedBsonConverter : OfferEventBsonConverter<OfferRemoved>
{
    protected override Guid GetOfferId(OfferRemoved @event) => @event.OfferId;

    protected override string GetEventParams(OfferRemoved @event) => string.Empty;
}
