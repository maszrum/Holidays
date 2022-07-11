using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;

namespace Holidays.InMemoryStore.EventHandlers;

public class OfferStartedTrackingInMemoryStoreEventHandler : IEventHandler<OfferStartedTracking>
{
    private readonly InMemoryStore _store;

    public OfferStartedTrackingInMemoryStoreEventHandler(InMemoryStore store)
    {
        _store = store;
    }

    public Task Handle(OfferStartedTracking @event, Func<Task> next, CancellationToken cancellationToken)
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
