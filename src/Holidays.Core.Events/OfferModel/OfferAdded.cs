using Holidays.Eventing.Core;

namespace Holidays.Core.Events.OfferModel;

public record OfferAdded(
    Guid OfferId,
    OfferData? OfferData,
    DateTime Timestamp) : IEvent
{
    public static OfferAdded WithOfferData(Guid offerId, OfferData offerData, DateTime timestamp) =>
        new(offerId, offerData, timestamp);

    public static OfferAdded WithoutOfferData(Guid offerId, DateTime timestamp) =>
        new(offerId, default, timestamp);
}
