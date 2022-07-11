using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferPriceChanged : IEvent
{
    public OfferPriceChanged(Guid offerId, int currentPrice, int previousPrice)
    {
        Offer = Maybe.Null<Offer>();
        OfferId = offerId;
        CurrentPrice = currentPrice;
        PreviousPrice = previousPrice;
    }

    public OfferPriceChanged(Offer offer, int previousPrice)
    {
        Offer = offer;
        OfferId = offer.Id;
        CurrentPrice = offer.Price;
        PreviousPrice = previousPrice;
    }
    
    public Maybe<Offer> Offer { get; }
    
    public Guid OfferId { get; }
    
    public int CurrentPrice { get; }
    
    public int PreviousPrice { get; }
}
