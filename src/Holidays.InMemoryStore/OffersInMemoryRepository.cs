using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.OfferModel;
using Holidays.InMemoryStore.Converters;

namespace Holidays.InMemoryStore;

public class OffersInMemoryRepository : IOffersRepository
{
    private readonly InMemoryDatabase _database;
    private readonly OfferDbRecordConverter _offerConverter = new();

    public OffersInMemoryRepository(InMemoryDatabase database)
    {
        _database = database;
    }

    public Task<Offers> GetAll()
    {
        var offers = _database.Offers
            .Select(_offerConverter.ConvertToObject);

        return Task.FromResult(new Offers(offers));
    }

    public Task<Offers> GetAllByWebsiteName(string websiteName)
    {
        var offers = _database.Offers
            .Where(r => r.WebsiteName == websiteName)
            .AsEnumerable()
            .Select(_offerConverter.ConvertToObject);

        return Task.FromResult(new Offers(offers));
    }

    public Task<Offers> GetAllRemovedByWebsiteName(string websiteName)
    {
        throw new InvalidOperationException(
            "In-memory repository does not store information about removed offers.");
    }
    
    public Task<Maybe<Offer>> Get(Guid offerId)
    {
        var record = _database.Offers.SingleOrDefault(o => o.Id == offerId);
        
        if (record is null)
        {
            return Task.FromResult(Maybe<Offer>.None());
        }

        var offer = _offerConverter.ConvertToObject(record);

        return Task.FromResult(Maybe.Some(offer));
    }

    public Task<Maybe<DateOnly>> GetLastDepartureDate(string websiteName)
    {
        var offersCount = _database.Offers.Count;
        
        if (offersCount == 0)
        {
            return Task.FromResult(Maybe.None<DateOnly>());
        }

        var result = _database.Offers
            .Where(o => o.WebsiteName == websiteName)
            .MaxBy(o => o.DepartureDate.DayNumber);

        return Task.FromResult(result is null 
            ? Maybe<DateOnly>.None() 
            : Maybe.Some(result.DepartureDate));
    }

    public void Add(Offer offer)
    {
        var existingOffer = _database.Offers.SingleOrDefault(o => o.Id == offer.Id);

        if (existingOffer is not null)
        {
            throw new InvalidOperationException(
                "Adding the offer failed: offer already exists.");
        }
        
        var record = _offerConverter.ConvertToRecord(offer);
        
        _database.Offers.Insert(record);
    }

    public void Remove(Guid offerId)
    {
        var currentRecord = _database.Offers.SingleOrDefault(o => o.Id == offerId);

        if (currentRecord is null)
        {
            throw new InvalidOperationException(
                "Removing the offer with specified id failed: offer does not exist.");
        }
        
        _database.Offers.Delete(currentRecord);
    }

    public int Count() => (int) _database.Offers.Count;

    public void ModifyPrice(Guid offerId, int price)
    {
        var currentRecord = _database.Offers.SingleOrDefault(o => o.Id == offerId);
        
        if (currentRecord is null)
        {
            throw new InvalidOperationException(
                "Offer with specified id does not exist.");
        }

        var newRecord = currentRecord with { Price = price };
        
        _database.Offers.Update(newRecord);
    }
}
