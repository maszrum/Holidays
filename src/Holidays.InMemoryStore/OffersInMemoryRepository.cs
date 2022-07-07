using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.OfferModel;
using Holidays.InMemoryStore.Converters;

namespace Holidays.InMemoryStore;

public class OffersInMemoryRepository : IOffersRepository
{
    private readonly InMemoryStore _store;
    private readonly OfferDbRecordConverter _offerConverter = new();

    public OffersInMemoryRepository(InMemoryStore store)
    {
        _store = store;
    }

    public Task<Offers> GetAll() => GetAll(getRemovedOffers: false);

    public Task<Offers> GetAllRemoved() => GetAll(getRemovedOffers: true);

    public Task<Maybe<Offer>> TryGetRemoved(Guid offerId)
    {
        if (!_store.Offers.TryGetValue(offerId, out var record) || record.IsRemoved)
        {
            return Task.FromResult(Maybe.Null<Offer>());
        }

        var offer = _offerConverter.ConvertToObject(record);

        return Task.FromResult(Maybe.Data(offer));
    }

    public void Add(Offer offer)
    {
        var record = _offerConverter.ConvertToRecord(offer, isRemoved: false);

        _store.Offers.AddOrUpdate(
            record.Id, 
            record, 
            (_, existingRecord) =>
            {
                if (!existingRecord.IsRemoved)
                {
                    throw new InvalidOperationException(
                        "Adding offer to in memory store failed: already exists.");
                }

                return record;
            });
    }

    public void Remove(Guid offerId)
    {
        if (!_store.Offers.TryGetValue(offerId, out var currentRecord) || currentRecord.IsRemoved)
        {
            throw new InvalidOperationException(
                "Removing offer with specified id failed: offer does not exist.");
        }

        var newRecord = currentRecord with { IsRemoved = true };

        if (!_store.Offers.TryUpdate(offerId, newRecord, currentRecord))
        {
            throw new InvalidOperationException(
                "Removing offer with specified id failed.");
        }
    }

    public void Modify(Offer offer)
    {
        var record = _offerConverter.ConvertToRecord(offer, isRemoved: false);
        
        var exists = _store.Offers.ContainsKey(record.Id);

        if (!exists)
        {
            throw new InvalidOperationException(
                "Offer with specified id does not exist.");
        }

        _store.Offers.AddOrUpdate(record.Id, record, (_, _) => record);
    }
    
    private Task<Offers> GetAll(bool getRemovedOffers)
    {
        var records = _store.Offers.Values;

        var offers = records
            .Where(r => r.IsRemoved == getRemovedOffers)
            .Select(_offerConverter.ConvertToObject);

        return Task.FromResult(new Offers(offers));
    }
}
