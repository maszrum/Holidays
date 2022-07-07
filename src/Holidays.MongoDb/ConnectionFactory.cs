using MongoDB.Bson;
using MongoDB.Driver;

namespace Holidays.MongoDb;

public class ConnectionFactory
{
    private IMongoDatabase? _database;
    private MongoClient? _client;

    private readonly MongoDbSettings _settings;

    public ConnectionFactory(MongoDbSettings settings)
    {
        _settings = settings;
    }

    public Task<IClientSessionHandle> StartSession() => 
        OpenConnectionOrGetExising().StartSessionAsync();

    public IMongoCollection<BsonDocument> GetOffersCollection() => 
        GetDocument(CollectionNames.Offers);

    public IMongoCollection<BsonDocument> GetOfferEventLogDocument() => 
        GetDocument(CollectionNames.OfferEventLog);

    private IMongoDatabase OpenDatabaseOrGetExistingConnection()
    {
        if (_database is null)
        {
            var client = OpenConnectionOrGetExising();
            _database = client.GetDatabase(_settings.Database);
        }

        return _database;
    }
    
    private MongoClient OpenConnectionOrGetExising()
    {
        return _client ??= new MongoClient(_settings.ConnectionString);
    }

    private IMongoCollection<BsonDocument> GetDocument(string name)
    {
        var database = OpenDatabaseOrGetExistingConnection();
        return database.GetCollection<BsonDocument>(name);
    }
}
