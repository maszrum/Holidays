using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;
using NMemory.Transactions;

namespace Holidays.InMemoryStore.EventHandlers;

public class OfferAddedInMemoryStoreEventHandler : IEventHandler<OfferAdded>
{
    private readonly InMemoryDatabase _database;

    public OfferAddedInMemoryStoreEventHandler(InMemoryDatabase database)
    {
        _database = database;
    }

    public async Task Handle(OfferAdded @event, Func<Task> next, CancellationToken cancellationToken)
    {
        if (!@event.Offer.TryGetData(out var offer))
        {
            throw new InvalidOperationException(
                "Received event without data.");
        }

        using var transaction = new TransactionContext();
        
        var repository = new OffersInMemoryRepository(_database);
        repository.Add(offer);

        await next();
            
        transaction.Complete();
    }
}
