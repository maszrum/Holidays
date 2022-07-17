using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;
using NMemory.Transactions;

namespace Holidays.InMemoryStore.EventHandlers;

public class OfferRemovedInMemoryStoreEventHandler : IEventHandler<OfferRemoved>
{
    private readonly InMemoryDatabase _database;

    public OfferRemovedInMemoryStoreEventHandler(InMemoryDatabase database)
    {
        _database = database;
    }

    public async Task Handle(OfferRemoved @event, Func<Task> next, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionContext();
        var repository = new OffersInMemoryRepository(_database);

        repository.Remove(@event.OfferId);

        await next();

        transaction.Complete();
    }
}
