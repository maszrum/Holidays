using Holidays.Core.OfferModel;
using Holidays.InMemoryStore.Converters;
using Holidays.InMemoryStore.DbRecords;
using NMemory;
using NMemory.Tables;

namespace Holidays.InMemoryStore;

public class InMemoryDatabase : Database
{
    private readonly ITable<OfferDbRecord> _offers;
    private readonly Func<Task<Offers>>? _initializationOffersFactory;

    private InMemoryDatabase()
    {
        _offers = Tables.Create<OfferDbRecord, Guid>(offer => offer.Id);
    }

    private InMemoryDatabase(Func<Task<Offers>> initializationOffersFactory) : this()
    {
        _initializationOffersFactory = initializationOffersFactory;
    }

    internal ITable<OfferDbRecord> Offers => IsInitialized
        ? _offers
        : throw new InvalidOperationException("Database is not initialized.");

    private bool IsInitialized { get; set; }

    public async Task Initialize()
    {
        ThrowIfAlreadyInitialized();

        if (_initializationOffersFactory is null)
        {
            throw new InvalidOperationException(
                "Initialization factory was not provided.");
        }

        var offers = await _initializationOffersFactory();

        InsertInitialState(offers);
    }

    private void InsertInitialState(Offers offersInitialState)
    {
        ThrowIfAlreadyInitialized();

        var converter = new OfferDbRecordConverter();

        var records = offersInitialState
            .Select(o => converter.ConvertToRecord(o));

        foreach (var offerRecord in records)
        {
            _offers.Insert(offerRecord);
        }

        IsInitialized = true;
    }

    private void ThrowIfAlreadyInitialized()
    {
        if (IsInitialized)
        {
            throw new InvalidOperationException(
                "Database has been initialized already.");
        }
    }

    public static InMemoryDatabase CreateWithInitialState(Offers offers)
    {
        var database = new InMemoryDatabase();
        database.InsertInitialState(offers);

        return database;
    }

    public static InMemoryDatabase CreateWithInitialStateFactory(Func<Task<Offers>> offersFactory) =>
        new(offersFactory);

    public static InMemoryDatabase CreateEmpty() =>
        new() { IsInitialized = true };
}
