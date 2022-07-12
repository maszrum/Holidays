using System.Transactions;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.OfferModel;
using Holidays.InMemoryStore.Converters;
using NMemory.Linq;
using NMemory.Transactions;

namespace Holidays.InMemoryStore;

public class OffersInMemoryRepository : IOffersRepository
{
    private readonly InMemoryDatabase _database;
    private readonly OfferDbRecordConverter _offerConverter = new();

    public OffersInMemoryRepository(InMemoryDatabase database)
    {
        _database = database;
    }

    public Task<Offers> GetAll() => GetAll(getRemovedOffers: false);

    public Task<Offers> GetAllRemoved() => GetAll(getRemovedOffers: true);
    
    public Task<Maybe<Offer>> Get(Guid offerId)
    {
        var record = _database.Offers.SingleOrDefault(o => o.Id == offerId);
        
        if (record is null || record.IsRemoved)
        {
            return Task.FromResult(Maybe<Offer>.None());
        }

        var offer = _offerConverter.ConvertToObject(record);

        return Task.FromResult(Maybe.Some(offer));
    }

    public Task<Maybe<DateOnly>> GetLastDepartureDate()
    {
        var offersCount = _database.Offers.Count;
        
        if (offersCount == 0)
        {
            return Task.FromResult(Maybe.None<DateOnly>());
        }

        var result = _database.Offers.MaxBy(o => o.DepartureDate.DayNumber);

        return Task.FromResult(Maybe.Some(result!.DepartureDate));
    }

    public Task<Maybe<Offer>> RemovedExists(Guid offerId)
    {
        var record = _database.Offers.SingleOrDefault(o => o.Id == offerId);
        
        if (record is null || !record.IsRemoved)
        {
            return Task.FromResult(Maybe.None<Offer>());
        }

        var offer = _offerConverter.ConvertToObject(record);

        return Task.FromResult(Maybe.Some(offer));
    }

    public void Add(Offer offer)
    {
        var existingOffer = _database.Offers.SingleOrDefault(o => o.Id == offer.Id);

        if (existingOffer is not null && !existingOffer.IsRemoved)
        {
            throw new InvalidOperationException(
                "Adding the offer failed: offer already exists.");
        }
        
        var record = _offerConverter.ConvertToRecord(offer, isRemoved: false);

        if (existingOffer is not null && existingOffer.IsRemoved)
        {
            _database.Offers.Update(record);
        }
        else
        {

            _database.Offers.Insert(record);
        }
    }

    public void Remove(Guid offerId)
    {
        var currentRecord = _database.Offers.SingleOrDefault(o => o.Id == offerId);

        if (currentRecord is null || currentRecord.IsRemoved)
        {
            throw new InvalidOperationException(
                "Removing the offer with specified id failed: offer does not exist.");
        }

        var newRecord = currentRecord with { IsRemoved = true };
        
        _database.Offers.Update(newRecord);
    }

    public int Count() => 
        _database.Offers.Count(o => !o.IsRemoved);

    public int CountRemoved() => 
        _database.Offers.Count(o => o.IsRemoved);

    public void ModifyPrice(Guid offerId, int price)
    {
        var currentRecord = _database.Offers.SingleOrDefault(o => o.Id == offerId);
        
        if (currentRecord is null || currentRecord.IsRemoved)
        {
            throw new InvalidOperationException(
                "Offer with specified id does not exist.");
        }

        var newRecord = currentRecord with { Price = price };
        
        _database.Offers.Update(newRecord);
    }

    private Task<Offers> GetAll(bool getRemovedOffers)
    {
        var offers = _database.Offers
            .Where(r => r.IsRemoved == getRemovedOffers)
            .AsEnumerable()
            .Select(_offerConverter.ConvertToObject);

        return Task.FromResult(new Offers(offers));
    }
}
