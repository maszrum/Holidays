using Holidays.Core.Events.OfferModel;
using Holidays.Core.OfferModel;

namespace Holidays.CollectingApp;

internal static class OfferExtensionMethods
{
    public static OfferStartedTracking ToStartedTrackingEvent(this Offer offer, DateTime timestamp)
    {
        return OfferStartedTracking.WithOfferData(
            offer.Id,
            ToOfferData(offer),
            timestamp);
    }

    public static OfferAdded ToOfferAddedEvent(this Offer offer, DateTime timestamp)
    {
        return OfferAdded.WithOfferData(
            offer.Id,
            ToOfferData(offer),
            timestamp);
    }

    private static OfferData ToOfferData(Offer offer)
    {
        return new OfferData(
            offer.Hotel,
            offer.Destination,
            offer.DepartureDate,
            offer.Days,
            offer.CityOfDeparture,
            offer.Price,
            offer.DetailsUrl,
            offer.WebsiteName);
    }
}
