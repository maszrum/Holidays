using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferAdded : IEvent
{
    public OfferAdded(Guid offerId, DateTime timestamp)
    {
        Offer = Maybe.Null<Offer>();
        OfferId = offerId;
        Timestamp = timestamp;
    }
    
    private OfferAdded(Offer offer, DateTime timestamp)
    {
        Offer = offer;
        OfferId = offer.Id;
        Timestamp = timestamp;
    }

    public Maybe<Offer> Offer { get; }
    
    public Guid OfferId { get; }
    
    public DateTime Timestamp { get; }

    public static OfferAdded ForNewOffer(Offer offer)
    {
        var timestamp = DateTime.UtcNow;
        return new OfferAdded(offer, timestamp);
    }
}
