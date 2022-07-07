using System.Collections.Concurrent;
using Holidays.Core.OfferModel;
using Holidays.InMemoryStore.Converters;
using Holidays.InMemoryStore.DbRecords;

namespace Holidays.InMemoryStore;

public class InMemoryStore
{
    internal ConcurrentDictionary<Guid, OfferDbRecord> Offers { get; }

    private InMemoryStore(IEnumerable<KeyValuePair<Guid, OfferDbRecord>> initialState)
    {
        Offers = new ConcurrentDictionary<Guid, OfferDbRecord>(initialState);
    }

    public static InMemoryStore CreateWithInitialState(Offers activeOffers, Offers removedOffers)
    {
        var converter = new OfferDbRecordConverter();

        var activeRecords = activeOffers
            .Select(o => converter.ConvertToRecord(o, isRemoved: false))
            .Select(r => KeyValuePair.Create(r.Id, r));

        var removedRecords = removedOffers
            .Select(o => converter.ConvertToRecord(o, isRemoved: true))
            .Select(r => KeyValuePair.Create(r.Id, r));

        return new InMemoryStore(activeRecords.Concat(removedRecords));
    }
}
