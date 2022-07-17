using Holidays.Eventing.Core;

namespace Holidays.Core.Events.OfferModel;

public record OfferAdded : IEvent
{
    private OfferAdded(
        Guid offerId,
        OfferData? offerData,
        DateTime timestamp)
    {
        OfferId = offerId;
        OfferData = offerData;
        Timestamp = timestamp;
    }

    public Guid OfferId { get; init; }

    public OfferData? OfferData { get; init; }

    public DateTime Timestamp { get; init; }

    public static OfferAdded WithOfferData(Guid offerId, OfferData offerData, DateTime timestamp) =>
        new(offerId, offerData, timestamp);

    public static OfferAdded WithoutOfferData(Guid offerId, DateTime timestamp) =>
        new(offerId, default, timestamp);
}
