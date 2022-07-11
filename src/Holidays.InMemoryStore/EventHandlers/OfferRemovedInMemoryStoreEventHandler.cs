using Holidays.Core.Eventing;
using Holidays.Core.OfferModel;

namespace Holidays.InMemoryStore.EventHandlers;

public class OfferRemovedInMemoryStoreEventHandler : IEventHandler<OfferRemoved>
{
    private readonly InMemoryStore _store;

    public OfferRemovedInMemoryStoreEventHandler(InMemoryStore store)
    {
        _store = store;
    }

    public Task Handle(OfferRemoved @event, Func<Task> next, CancellationToken cancellationToken)
    {
        var repository = new OffersInMemoryRepository(_store);

        repository.Remove(@event.OfferId);

        return next();
    }
}
