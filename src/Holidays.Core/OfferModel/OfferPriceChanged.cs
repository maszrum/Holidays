using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferPriceChanged : IEvent
{
    public OfferPriceChanged(
        Guid offerId, 
        int currentPrice, 
        int previousPrice, 
        DateTime timestamp)
    {
        Offer = Maybe.Null<Offer>();
        OfferId = offerId;
        CurrentPrice = currentPrice;
        PreviousPrice = previousPrice;
        Timestamp = timestamp;
    }

    public OfferPriceChanged(
        Offer offer, 
        int previousPrice, 
        DateTime timestamp)
    {
        Offer = offer;
        OfferId = offer.Id;
        CurrentPrice = offer.Price;
        PreviousPrice = previousPrice;
        Timestamp = timestamp;
    }
    
    public Maybe<Offer> Offer { get; }
    
    public Guid OfferId { get; }
    
    public int CurrentPrice { get; }
    
    public int PreviousPrice { get; }
    
    public DateTime Timestamp { get; }
}
