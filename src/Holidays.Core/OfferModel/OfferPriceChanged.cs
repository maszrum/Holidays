using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferPriceChanged(Guid OfferId, 
    int CurrentPrice, 
    int PreviousPrice, 
    DateTime Timestamp) : IEvent;
