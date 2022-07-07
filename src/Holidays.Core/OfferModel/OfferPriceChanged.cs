using Holidays.Core.Eventing;

namespace Holidays.Core.OfferModel;

public record OfferPriceChanged(Offer Offer, int PreviousPrice) : IEvent;
