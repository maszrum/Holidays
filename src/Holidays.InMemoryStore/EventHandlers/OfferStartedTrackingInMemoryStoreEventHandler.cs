using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;
using NMemory.Transactions;

namespace Holidays.InMemoryStore.EventHandlers;

public class OfferStartedTrackingInMemoryStoreEventHandler : IEventHandler<OfferStartedTracking>
{
    private readonly InMemoryDatabase _database;

    public OfferStartedTrackingInMemoryStoreEventHandler(InMemoryDatabase database)
    {
        _database = database;
    }

    public async Task Handle(OfferStartedTracking @event, Func<Task> next, CancellationToken cancellationToken)
    {
        if (@event.Offer is null)
        {
            throw new InvalidOperationException(
                "Received event without data.");
        }

        using var transaction = new TransactionContext();

        var repository = new OffersInMemoryRepository(_database);

        repository.Add(@event.Offer);

        await next();

        transaction.Complete();
    }
}
