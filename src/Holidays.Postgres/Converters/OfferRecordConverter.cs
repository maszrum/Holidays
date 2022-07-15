using Holidays.Core.OfferModel;
using Holidays.Postgres.DbRecords;

namespace Holidays.Postgres.Converters;

internal class OfferRecordConverter
{
    public OfferDbRecord ConvertToRecord(Offer offer, bool isRemoved)
    {
        return new OfferDbRecord(
            offer.Id,
            offer.Hotel,
            offer.Destination,
            offer.DepartureDate.DayNumber,
            offer.Days,
            offer.CityOfDeparture,
            offer.Price,
            offer.DetailsUrl,
            offer.WebsiteName,
            isRemoved);
    }

    public Offer ConvertToObject(OfferDbRecord record)
    {
        var offer = new Offer(
            record.Hotel,
            record.Destination,
            DateOnly.FromDayNumber(record.DepartureDate),
            record.Days,
            record.CityOfDeparture,
            record.Price,
            record.DetailsUrl,
            record.WebsiteName);

        if (offer.Id != record.Id)
        {
            throw new InvalidOperationException(
                "Invalid object id when converting from bson format.");
        }

        return offer;
    }
}
