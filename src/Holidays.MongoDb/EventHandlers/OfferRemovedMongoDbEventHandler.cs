using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;
using Holidays.MongoDb.Converters;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Holidays.MongoDb.EventHandlers;

public class OfferRemovedMongoDbEventHandler : IEventHandler<OfferRemoved>
{
    private readonly ConnectionFactory _connectionFactory;

    public OfferRemovedMongoDbEventHandler(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Handle(OfferRemoved @event, CancellationToken cancellationToken)
    {
        await RemoveFromOffers(@event.OfferId, cancellationToken);
        await InsertIntoOfferEventLog(@event, cancellationToken);
    }

    private async Task RemoveFromOffers(Guid offerId, CancellationToken cancellationToken)
    {
        var collection = _connectionFactory.GetOffersCollection();
        
        await collection.DeleteOneAsync(
            Builders<BsonDocument>.Filter.Eq("id", offerId.ToString()), 
            cancellationToken);
    }

    private async Task InsertIntoOfferEventLog(OfferRemoved @event, CancellationToken cancellationToken)
    {
        var converter = new OfferRemovedBsonConverter();
        var bson = converter.ConvertToBson(@event);
        
        var collection = _connectionFactory.GetOfferEventLogDocument();
        await collection.InsertOneAsync(bson, default, cancellationToken);
    }
}
