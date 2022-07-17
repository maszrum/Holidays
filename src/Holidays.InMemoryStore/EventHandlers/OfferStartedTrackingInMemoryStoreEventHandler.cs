using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;
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
        if (@event.OfferData is null)
        {
            throw new InvalidOperationException(
                "Received event without offer data.");
        }

        var offer = @event.ToOffer();

        using var transaction = new TransactionContext();
        var repository = new OffersInMemoryRepository(_database);

        repository.Add(offer);

        await next();

        transaction.Complete();
    }
}
