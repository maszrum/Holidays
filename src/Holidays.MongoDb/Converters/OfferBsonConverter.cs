using System.Globalization;
using Holidays.Core.OfferModel;
using MongoDB.Bson;

namespace Holidays.MongoDb.Converters;

internal class OfferBsonConverter
{
    public BsonDocument ConvertToBson(Offer offer, bool isRemoved)
    {
        return new BsonDocument
        {
            { "id", new BsonString(offer.Id.ToString()) },
            { "hotel", new BsonString(offer.Hotel) },
            { "destination", new BsonString(offer.Destination) },
            { "departureDate", new BsonString(offer.DepartureDate.ToString(CultureInfo.InvariantCulture)) },
            { "days", new BsonInt32(offer.Days) },
            { "cityOfDeparture", new BsonString(offer.CityOfDeparture) },
            { "price", new BsonInt32(offer.Price) },
            { "detailsUrl", new BsonString(offer.DetailsUrl) },
            { "isRemoved", new BsonBoolean(isRemoved) }
        };
    }

    public Offer ConvertToObject(BsonDocument bson)
    {
        var offer = new Offer(
            hotel: bson["hotel"].AsString,
            destination: bson["destination"].AsString,
            departureDate: DateOnly.Parse(bson["departureDate"].AsString, CultureInfo.InvariantCulture),
            days: bson["days"].AsInt32,
            cityOfDeparture: bson["cityOfDeparture"].AsString,
            price: bson["price"].AsInt32,
            detailsUrl: bson["detailsUrl"].AsString);

        var idFromDb = Guid.Parse(bson["id"].AsString);

        if (offer.Id != idFromDb)
        {
            throw new InvalidOperationException(
                "Invalid object id when converting from bson format.");
        }

        return offer;
    }
}
