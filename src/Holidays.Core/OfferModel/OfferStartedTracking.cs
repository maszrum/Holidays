using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferStartedTracking(
    Offer? Offer, 
    Guid OfferId, 
    DateTime Timestamp) : IEvent;
