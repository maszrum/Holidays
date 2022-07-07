using MongoDB.Bson;

namespace Holidays.MongoDb.Converters;

internal abstract class OfferEventBsonConverter<TEvent>
{
    public BsonDocument ConvertToBson(TEvent @event)
    {
        return new BsonDocument
        {
            { "eventType", new BsonString(typeof(TEvent).Name) },
            { "offerId", new BsonString(GetOfferId(@event).ToString()) },
            { "params", new BsonString(GetEventParams(@event)) }
        };
    }

    protected abstract Guid GetOfferId(TEvent @event);

    protected abstract string GetEventParams(TEvent @event);
}
