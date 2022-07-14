using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferAdded(
    Offer? Offer, 
    Guid OfferId, 
    DateTime Timestamp) : IEvent;
