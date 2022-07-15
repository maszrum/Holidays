using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;
using NMemory.Transactions;

namespace Holidays.InMemoryStore.EventHandlers;

public class OfferPriceChangedInMemoryStoreEventHandler : IEventHandler<OfferPriceChanged>
{
    private readonly InMemoryDatabase _database;

    public OfferPriceChangedInMemoryStoreEventHandler(InMemoryDatabase database)
    {
        _database = database;
    }

    public async Task Handle(OfferPriceChanged @event, Func<Task> next, CancellationToken cancellationToken)
    {
        using var transaction = new TransactionContext();

        var repository = new OffersInMemoryRepository(_database);
        repository.ModifyPrice(@event.OfferId, @event.CurrentPrice);

        await next();

        transaction.Complete();
    }
}
