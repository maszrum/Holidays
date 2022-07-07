using Holidays.Core.OfferModel;

namespace Holidays.MongoDb.Converters;

internal class OfferAddedBsonConverter : OfferEventBsonConverter<OfferAdded>
{
    protected override Guid GetOfferId(OfferAdded @event) => @event.Offer.Id;

    protected override string GetEventParams(OfferAdded @event) => string.Empty;
}
