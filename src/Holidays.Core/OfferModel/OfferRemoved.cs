using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferRemoved(
    Guid OfferId, 
    DateTime Timestamp) : IEvent;
