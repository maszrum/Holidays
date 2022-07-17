using Holidays.Eventing.Core;

namespace Holidays.Core.Events.OfferModel;

public record OfferPriceChanged(
    Guid OfferId,
    int CurrentPrice,
    int PreviousPrice,
    DateTime Timestamp) : IEvent;
