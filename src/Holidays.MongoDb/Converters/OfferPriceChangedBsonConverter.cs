using Holidays.Core.OfferModel;

namespace Holidays.MongoDb.Converters;

internal class OfferPriceChangedBsonConverter : OfferEventBsonConverter<OfferPriceChanged>
{
    protected override Guid GetOfferId(OfferPriceChanged @event) => 
        @event.Offer.Id;

    protected override string GetEventParams(OfferPriceChanged @event) => 
        $"{@event.PreviousPrice},{@event.Offer.Price}";
}
