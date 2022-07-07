using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;

namespace Holidays.InMemoryStore.EventHandlers;

public class OfferPriceChangedInMemoryStoreEventHandler : IEventHandler<OfferPriceChanged>
{
    private readonly InMemoryStore _inMemoryStore;

    public OfferPriceChangedInMemoryStoreEventHandler(InMemoryStore inMemoryStore)
    {
        _inMemoryStore = inMemoryStore;
    }

    public Task Handle(OfferPriceChanged @event, CancellationToken cancellationToken)
    {
        var repository = new OffersInMemoryRepository(_inMemoryStore);
        
        repository.Modify(@event.Offer);

        return Task.CompletedTask;
    }
}
