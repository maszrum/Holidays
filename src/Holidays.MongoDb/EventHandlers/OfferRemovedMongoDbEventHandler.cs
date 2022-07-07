using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;

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
        var session = await _connectionFactory.StartSession();

        using var offersRepository = new OffersMongoRepository(_connectionFactory, session);
        using var offerChangesRepository = new OfferChangesMongoRepository(_connectionFactory, session);
        
        session.StartTransaction();

        try
        {
            await offersRepository.Remove(@event.OfferId, cancellationToken);
            await offerChangesRepository.Add(@event, cancellationToken);

            await session.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await session.AbortTransactionAsync(CancellationToken.None);
        }
    }
}
