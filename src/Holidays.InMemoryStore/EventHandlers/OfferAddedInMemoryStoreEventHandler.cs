using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;

namespace Holidays.InMemoryStore.EventHandlers;

public class OfferAddedInMemoryStoreEventHandler : IEventHandler<OfferAdded>
{
    private readonly InMemoryStore _store;

    public OfferAddedInMemoryStoreEventHandler(InMemoryStore store)
    {
        _store = store;
    }

    public Task Handle(OfferAdded @event, CancellationToken cancellationToken)
    {
        var repository = new OffersInMemoryRepository(_store);
        
        repository.Add(@event.Offer);

        return Task.CompletedTask;
    }
}
