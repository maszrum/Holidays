using Holidays.Core.OfferModel;
using Holidays.InMemoryStore.Converters;
using Holidays.InMemoryStore.DbRecords;
using NMemory;
using NMemory.Tables;

namespace Holidays.InMemoryStore;

public class InMemoryDatabase : Database
{
    private InMemoryDatabase()
    {
        Offers = Tables.Create<OfferDbRecord, Guid>(offer => offer.Id);
    }

    internal ITable<OfferDbRecord> Offers { get; }

    private void InsertInitialState(IEnumerable<OfferDbRecord> initialState)
    {
        foreach (var offerRecord in initialState)
        {
            Offers.Insert(offerRecord);
        }
    }

    public static InMemoryDatabase CreateWithInitialState(Offers activeOffers, Offers removedOffers)
    {
        var converter = new OfferDbRecordConverter();

        var activeRecords = activeOffers
            .Select(o => converter.ConvertToRecord(o, isRemoved: false));

        var removedRecords = removedOffers
            .Select(o => converter.ConvertToRecord(o, isRemoved: true));

        var database = new InMemoryDatabase();
        
        database.InsertInitialState(activeRecords.Concat(removedRecords));

        return database;
    }

    public static InMemoryDatabase CreateEmpty() => new();
}
