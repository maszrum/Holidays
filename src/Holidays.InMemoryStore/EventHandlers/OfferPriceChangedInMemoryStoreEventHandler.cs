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
        if (!@event.Offer.TryGetData(out var offer))
        {
            throw new InvalidOperationException(
                "Received event without data.");
        }
        
        var repository = new OffersInMemoryRepository(_inMemoryStore);
        
        repository.Modify(offer);

        return Task.CompletedTask;
    }
}
