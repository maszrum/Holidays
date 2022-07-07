using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.OfferModel;
using Holidays.MongoDb.Converters;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Holidays.MongoDb;

public class OffersMongoRepository : IOffersRepository
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly OfferBsonConverter _offerConverter = new();

    public OffersMongoRepository(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public Task<Offers> GetAll() => GetAll(getRemovedOffers: false);

    public Task<Offers> GetAllRemoved() => GetAll(getRemovedOffers: true);

    public async Task<Maybe<Offer>> TryGetRemoved(Guid offerId)
    {
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq("id", offerId.ToString()),
            Builders<BsonDocument>.Filter.Eq("isRemoved", true));
        
        var cursor = await _connectionFactory.GetOffersCollection()
            .FindAsync(filter);

        var bsonDocument = await cursor.SingleOrDefaultAsync();

        if (bsonDocument is null)
        {
            return default(Offer);
        }

        var offer = _offerConverter.ConvertToObject(bsonDocument);

        return offer;
    }
    
    private async Task<Offers> GetAll(bool getRemovedOffers)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("isRemoved", getRemovedOffers);
        
        var cursor = await _connectionFactory.GetOffersCollection()
            .FindAsync(filter);

        var bsonDocuments = await cursor.ToListAsync();

        var offers = bsonDocuments.Select(_offerConverter.ConvertToObject);

        return new Offers(offers);
    }
}
