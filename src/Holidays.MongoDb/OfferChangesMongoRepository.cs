using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.OfferModel;
using Holidays.MongoDb.Converters;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Holidays.MongoDb;

internal sealed class OfferChangesMongoRepository : MongoRepositoryBase, IOfferChangesRepository
{
    public OfferChangesMongoRepository(ConnectionFactory connectionFactory) 
        : base(connectionFactory)
    {
    }

    public OfferChangesMongoRepository(ConnectionFactory connectionFactory, IClientSessionHandle session) 
        : base(connectionFactory, session)
    {
    }
    
    public Task Add(OfferAdded @event, CancellationToken cancellationToken)
    {
        var converter = new OfferAddedBsonConverter();
        var bson = converter.ConvertToBson(@event);

        return AddOfferChange(bson, cancellationToken);
    }

    public Task Add(OfferPriceChanged @event, CancellationToken cancellationToken)
    {
        var converter = new OfferPriceChangedBsonConverter();
        var bson = converter.ConvertToBson(@event);

        return AddOfferChange(bson, cancellationToken);
    }

    public Task Add(OfferRemoved @event, CancellationToken cancellationToken)
    {
        var converter = new OfferRemovedBsonConverter();
        var bson = converter.ConvertToBson(@event);

        return AddOfferChange(bson, cancellationToken);
    }

    private async Task AddOfferChange(BsonDocument bsonDocument, CancellationToken cancellationToken)
    {
        var session = await GetSession();
        
        var collection = ConnectionFactory.GetOfferEventLogDocument();
        await collection.InsertOneAsync(session, bsonDocument, default, cancellationToken);
    }
}
