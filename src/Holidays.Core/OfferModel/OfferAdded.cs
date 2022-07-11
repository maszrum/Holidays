using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferAdded : IEvent
{
    public OfferAdded(Guid offerId)
    {
        Offer = Maybe.Null<Offer>();
        OfferId = offerId;
    }
    
    public OfferAdded(Offer offer)
    {
        Offer = offer;
        OfferId = offer.Id;
    }

    public Maybe<Offer> Offer { get; }
    
    public Guid OfferId { get; }
}
