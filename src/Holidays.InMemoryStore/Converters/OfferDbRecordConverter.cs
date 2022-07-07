using Holidays.Core.OfferModel;
using Holidays.InMemoryStore.DbRecords;

namespace Holidays.InMemoryStore.Converters;

internal class OfferDbRecordConverter
{
    public Offer ConvertToObject(OfferDbRecord record)
    {
        return new Offer(
            record.Hotel,
            record.Destination,
            record.DepartureDate,
            record.Days,
            record.CityOfDeparture,
            record.Price,
            record.DetailsUrl);
    }

    public OfferDbRecord ConvertToRecord(Offer offer, bool isRemoved)
    {
        return new OfferDbRecord(
            offer.Id,
            offer.Hotel,
            offer.Destination,
            offer.DepartureDate,
            offer.Days,
            offer.CityOfDeparture,
            offer.Price,
            offer.DetailsUrl,
            isRemoved);
    }
}
