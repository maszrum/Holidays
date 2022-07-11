namespace Holidays.Core.OfferModel;

public class PriceHistory
{

    public PriceHistory(Guid offerId, IReadOnlyDictionary<DateTime, int> prices)
    {
        OfferId = offerId;
        Prices = prices;
    }
    
    public Guid OfferId { get; }
    
    public IReadOnlyDictionary<DateTime, int> Prices { get; }
}
