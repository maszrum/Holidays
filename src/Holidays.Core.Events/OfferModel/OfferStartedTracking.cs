using Holidays.Eventing.Core;

namespace Holidays.Core.Events.OfferModel;

public record OfferStartedTracking : IEvent
{
    private OfferStartedTracking(
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

    public static OfferStartedTracking WithOfferData(Guid offerId, OfferData offerData, DateTime timestamp) =>
        new(offerId, offerData, timestamp);

    public static OfferStartedTracking WithoutOfferData(Guid offerId, DateTime timestamp) =>
        new(offerId, default, timestamp);
}
