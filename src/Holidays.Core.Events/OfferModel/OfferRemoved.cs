using Holidays.Eventing.Core;

namespace Holidays.Core.Events.OfferModel;

public record OfferRemoved(
    Guid OfferId,
    DateTime Timestamp) : IEvent;
