using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferStartedTracking : IEvent
{
    public OfferStartedTracking(Guid offerId, DateTime timestamp)
    {
        Offer = Maybe<Offer>.None();
        OfferId = offerId;
        Timestamp = timestamp;
    }
    
    private OfferStartedTracking(Offer offer, DateTime timestamp)
    {
        Offer = Maybe.Some(offer);
        OfferId = offer.Id;
        Timestamp = timestamp;
    }

    public Maybe<Offer> Offer { get; }
    
    public Guid OfferId { get; }

    public DateTime Timestamp { get; }

    public static OfferStartedTracking ForNewOffer(Offer offer)
    {
        var timestamp = DateTime.UtcNow;
        return new OfferStartedTracking(offer, timestamp);
    }
}
