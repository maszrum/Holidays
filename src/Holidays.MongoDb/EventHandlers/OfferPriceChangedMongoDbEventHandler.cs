using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;
using Holidays.MongoDb.Converters;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Holidays.MongoDb.EventHandlers;

public class OfferPriceChangedMongoDbEventHandler : IEventHandler<OfferPriceChanged>
{
    private readonly ConnectionFactory _connectionFactory;

    public OfferPriceChangedMongoDbEventHandler(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Handle(OfferPriceChanged @event, CancellationToken cancellationToken)
    {
        await ModifyInOffers(@event.Offer.Id, @event.Offer.Price, cancellationToken);
        await InsertIntoOfferEventLog(@event, cancellationToken);
    }

    private async Task ModifyInOffers(Guid offerId, int newPrice, CancellationToken cancellationToken)
    {
        var collection = _connectionFactory.GetOffersCollection();

        await collection.UpdateOneAsync(
            Builders<BsonDocument>.Filter.Eq("id", offerId.ToString()),
            Builders<BsonDocument>.Update.Set("price", newPrice),
            default,
            cancellationToken);
    }

    private async Task InsertIntoOfferEventLog(OfferPriceChanged @event, CancellationToken cancellationToken)
    {
        var converter = new OfferPriceChangedBsonConverter();
        var bson = converter.ConvertToBson(@event);
        
        var collection = _connectionFactory.GetOfferEventLogDocument();
        await collection.InsertOneAsync(bson, default, cancellationToken);
    }
}
