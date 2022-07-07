using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.OfferModel;
using Holidays.MongoDb.Converters;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Holidays.MongoDb;

public sealed class OffersMongoRepository : MongoRepositoryBase, IOffersRepository
{
    private readonly OfferBsonConverter _offerConverter = new();
    public OffersMongoRepository(ConnectionFactory connectionFactory) 
        : base(connectionFactory)
    {
    }

    public OffersMongoRepository(ConnectionFactory connectionFactory, IClientSessionHandle session) 
        : base(connectionFactory, session)
    {
    }

    public Task<Offers> GetAll() => GetAll(getRemovedOffers: false);

    public Task<Offers> GetAllRemoved() => GetAll(getRemovedOffers: true);

    public async Task<Maybe<Offer>> TryGetRemoved(Guid offerId)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("id", offerId.ToString()),
            Builders<BsonDocument>.Filter.Eq("isRemoved", true));

        var session = await GetSession();
        
        var cursor = await ConnectionFactory.GetOffersCollection()
            .FindAsync(session, filter);

        var bsonDocument = await cursor.SingleOrDefaultAsync();

        if (bsonDocument is null)
        {
            return default(Offer);
        }

        var offer = _offerConverter.ConvertToObject(bsonDocument);

        return offer;
    }

    public async Task Add(Offer offer, CancellationToken cancellationToken)
    {
        var bson = _offerConverter.ConvertToBson(offer, isRemoved: false);

        var collection = ConnectionFactory.GetOffersCollection();

        var session = await GetSession();
        
        await collection.InsertOneAsync(session, bson, default, cancellationToken);
    }

    public async Task ModifyPrice(Guid offerId, int newPrice, CancellationToken cancellationToken)
    {
        var collection = ConnectionFactory.GetOffersCollection();

        var session = await GetSession();

        await collection.UpdateOneAsync(
            session,
            Builders<BsonDocument>.Filter.Eq("id", offerId.ToString()),
            Builders<BsonDocument>.Update.Set("price", newPrice),
            default,
            cancellationToken);
    }
    
    public async Task Remove(Guid offerId, CancellationToken cancellationToken)
    {
        var collection = ConnectionFactory.GetOffersCollection();

        var session = await GetSession();
        
        await collection.DeleteOneAsync(
            session,
            Builders<BsonDocument>.Filter.Eq("id", offerId.ToString()), 
            default,
            cancellationToken);
    }
    
    private async Task<Offers> GetAll(bool getRemovedOffers)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("isRemoved", getRemovedOffers);

        var session = await GetSession();
        
        var cursor = await ConnectionFactory.GetOffersCollection()
            .FindAsync(session, filter);

        var bsonDocuments = await cursor.ToListAsync();

        var offers = bsonDocuments.Select(_offerConverter.ConvertToObject);

        return new Offers(offers);
    }
}
