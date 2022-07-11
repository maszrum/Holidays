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

    public Task Handle(OfferAdded @event, Func<Task> next, CancellationToken cancellationToken)
    {
        if (!@event.Offer.TryGetData(out var offer))
        {
            throw new InvalidOperationException(
                "Received event without data.");
        }
        
        var repository = new OffersInMemoryRepository(_store);
        
        repository.Add(offer);

        return next();
    }
}
