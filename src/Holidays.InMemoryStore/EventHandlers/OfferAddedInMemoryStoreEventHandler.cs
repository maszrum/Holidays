using Holidays.Core.Events.OfferModel;
using Holidays.Core.OfferModel;
using Holidays.Eventing.Core;
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
