using Holidays.Core.OfferModel;
using Holidays.InMemoryStore.DbRecords;

namespace Holidays.InMemoryStore.Converters;

internal class OfferDbRecordConverter
{
    public Offer ConvertToObject(OfferDbRecord record)
    {
        return new Offer(
            record.Hotel,
            record.DestinationCountry,
            record.DetailedDestination,
            record.DepartureDate,
            record.Days,
            record.CityOfDeparture,
            record.Price,
            record.DetailsUrl,
            record.WebsiteName);
    }

    public OfferDbRecord ConvertToRecord(Offer offer)
    {
        return new OfferDbRecord(
            offer.Id,
            offer.Hotel,
            offer.DestinationCountry,
            offer.DetailedDestination,
            offer.DepartureDate,
            offer.Days,
            offer.CityOfDeparture,
            offer.Price,
            offer.DetailsUrl,
            offer.WebsiteName);
    }
}
