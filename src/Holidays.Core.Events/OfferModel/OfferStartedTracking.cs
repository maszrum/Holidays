using Holidays.Eventing.Core;

namespace Holidays.Core.Events.OfferModel;

public record OfferStartedTracking(
    Guid OfferId,
    OfferData? OfferData,
    DateTime Timestamp) : IEvent
{
    public static OfferStartedTracking WithOfferData(Guid offerId, OfferData offerData, DateTime timestamp) =>
        new(offerId, offerData, timestamp);

    public static OfferStartedTracking WithoutOfferData(Guid offerId, DateTime timestamp) =>
        new(offerId, default, timestamp);
}
