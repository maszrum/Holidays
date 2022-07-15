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

    public static InMemoryDatabase CreateWithInitialState(Offers offers)
    {
        var converter = new OfferDbRecordConverter();

        var records = offers
            .Select(o => converter.ConvertToRecord(o));

        var database = new InMemoryDatabase();
        
        database.InsertInitialState(records);

        return database;
    }

    public static InMemoryDatabase CreateEmpty() => new();
}
