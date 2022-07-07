using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;
using Holidays.MongoDb.Converters;

namespace Holidays.MongoDb.EventHandlers;

public class OfferAddedMongoDbEventHandler : IEventHandler<OfferAdded>
{
    private readonly ConnectionFactory _connectionFactory;

    public OfferAddedMongoDbEventHandler(ConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Handle(OfferAdded @event, CancellationToken cancellationToken)
    {
        await InsertIntoOffers(@event.Offer, cancellationToken);
        await InsertIntoOfferEventLog(@event, cancellationToken);
    }

    private async Task InsertIntoOffers(Offer offer, CancellationToken cancellationToken)
    {
        var converter = new OfferBsonConverter();
        var bson = converter.ConvertToBson(offer, isRemoved: false);

        var collection = _connectionFactory.GetOffersCollection();
        await collection.InsertOneAsync(bson, default, cancellationToken);
    }

    private async Task InsertIntoOfferEventLog(OfferAdded @event, CancellationToken cancellationToken)
    {
        var converter = new OfferAddedBsonConverter();
        var bson = converter.ConvertToBson(@event);
        
        var collection = _connectionFactory.GetOfferEventLogDocument();
        await collection.InsertOneAsync(bson, default, cancellationToken);
    }
}
