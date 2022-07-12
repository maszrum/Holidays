using Holidays.Core.OfferModel;
using NUnit.Framework;

namespace Holidays.InMemoryStore.Tests.TestFixtures;

[TestFixture]
public class OffersInMemoryRepositoryTests : DatabaseTestsBase
{
    [Test]
    public async Task add_one_offer_and_check_if_added_and_if_valid_data_returned()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(4), 7, "city", 1300, "url", "website");

        var getOffers = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);

            repository.Add(offer);

            var offers = await repository.GetAll();
            return offers;
        });
        
        Assert.That(getOffers.Elements, Has.Count.EqualTo(1));

        var getOffer = getOffers.First();

        Assert.That(getOffer.Id, Is.EqualTo(offer.Id));
        Assert.That(getOffer.Hotel, Is.EqualTo("hotel"));
        Assert.That(getOffer.Destination, Is.EqualTo("destination"));
        Assert.That(getOffer.DepartureDate, Is.EqualTo(DateOnly.FromDayNumber(4)));
        Assert.That(getOffer.Days, Is.EqualTo(7));
        Assert.That(getOffer.CityOfDeparture, Is.EqualTo("city"));
        Assert.That(getOffer.Price, Is.EqualTo(1300));
        Assert.That(getOffer.DetailsUrl, Is.EqualTo("url"));
        Assert.That(getOffer.WebsiteName, Is.EqualTo("website"));
    }

    [Test]
    public async Task add_many_offers_and_check_if_count_method_return_correct_value()
    {
        var offersToAdd = Enumerable
            .Range(1, 20)
            .Select(i =>
                new Offer($"hotel-{i}", "destination", DateOnly.FromDayNumber(4), 7, "city", 1300, "url", "website"))
            .ToArray();

        var getOffers = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);

            foreach (var offerToAdd in offersToAdd)
            {
                repository.Add(offerToAdd);
            }

            var offers = await repository.GetAll();
            return offers;
        });
        
        Assert.That(getOffers.Elements, Has.Count.EqualTo(20));

        CollectionAssert.AreEquivalent(
            Enumerable.Range(1, 20).Select(i => $"hotel-{i}"),
            getOffers.Select(o => o.Hotel));
    }

    [Test]
    public async Task add_three_offers_remove_one_and_check_if_offer_does_not_exist()
    {
        var offersToAdd = Enumerable
            .Range(1, 3)
            .Select(i =>
                new Offer($"hotel-{i}", "destination", DateOnly.FromDayNumber(4), 7, "city", 1300, "url", "website"))
            .ToArray();

        var getOffers = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);

            foreach (var offerToAdd in offersToAdd)
            {
                repository.Add(offerToAdd);
            }

            repository.Remove(offersToAdd[1].Id);

            var offers = await repository.GetAll();
            return offers;
        });

        Assert.That(getOffers.Elements, Has.Count.EqualTo(2));

        CollectionAssert.AreEquivalent(
            new[] { "hotel-1", "hotel-3" },
            getOffers.Select(o => o.Hotel));
    }

    [Test]
    public async Task add_three_offers_remove_one_and_check_if_exists_in_removed_offers()
    {
        var offersToAdd = Enumerable
            .Range(1, 3)
            .Select(i =>
                new Offer($"hotel-{i}", "destination", DateOnly.FromDayNumber(4), 7, "city", 1300, "url", "website"))
            .ToArray();

        var getOffers = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);

            foreach (var offerToAdd in offersToAdd)
            {
                repository.Add(offerToAdd);
            }

            repository.Remove(offersToAdd[1].Id);

            var offers = await repository.GetAllRemovedByWebsiteName("website");
            return offers;
        });

        Assert.That(getOffers.Elements, Has.Count.EqualTo(1));

        Assert.That(getOffers.Elements.First().Hotel, Is.EqualTo("hotel-2"));
    }

    [Test]
    public async Task add_many_offers_remove_them_and_check_if_count_removed_offers_returns_correct_value()
    {
        var offersToAddAndRemove = Enumerable
            .Range(1, 23)
            .Select(i =>
                new Offer($"hotel-{i}", "destination", DateOnly.FromDayNumber(4), 7, "city", 1300, "url", "website"))
            .ToArray();

        var getOffers = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);

            foreach (var offerToAdd in offersToAddAndRemove)
            {
                repository.Add(offerToAdd);
            }

            foreach (var offerToRemove in offersToAddAndRemove)
            {
                repository.Remove(offerToRemove.Id);
            }

            var offers = await repository.GetAllRemovedByWebsiteName("website");
            return offers;
        });

        Assert.That(getOffers.Elements, Has.Count.EqualTo(23));

        CollectionAssert.AreEquivalent(
            Enumerable.Range(1, 23).Select(i => $"hotel-{i}"),
            getOffers.Select(o => o.Hotel));
    }

    [Test]
    public async Task get_all_offers_should_return_empty_collection()
    {
        var getOffers = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);
            
            var offers = await repository.GetAll();
            return offers;
        });

        Assert.That(getOffers.Elements, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task get_all_removed_offers_should_return_empty_collection()
    {
        var getOffers = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);
            
            var offers = await repository.GetAllRemovedByWebsiteName("website");
            return offers;
        });

        Assert.That(getOffers.Elements, Has.Count.EqualTo(0));
    }

    [Test]
    public void add_remove_add_offer_should_work_correctly()
    {
        var offer = new Offer($"hotel", "destination", DateOnly.FromDayNumber(4), 7, "city", 1300, "url", "website");

        var offersCount = new List<int>();
        var offersRemovedCount = new List<int>();

        DoWithTransactionAndRollback(database =>
        {
            var repository = new OffersInMemoryRepository(database);

            offersCount.Add(repository.Count());
            offersRemovedCount.Add(repository.CountRemoved());

            repository.Add(offer);

            offersCount.Add(repository.Count());
            offersRemovedCount.Add(repository.CountRemoved());

            repository.Remove(offer.Id);

            offersCount.Add(repository.Count());
            offersRemovedCount.Add(repository.CountRemoved());

            repository.Add(offer);

            offersCount.Add(repository.Count());
            offersRemovedCount.Add(repository.CountRemoved());
        });
        
        CollectionAssert.AreEqual(
            new[] { 0, 1, 0, 1 },
            offersCount);

        CollectionAssert.AreEqual(
            new[] { 0, 0, 1, 0 },
            offersRemovedCount);
    }

    [Test]
    public void adding_same_offer_two_times_should_throw_exception()
    {
        var offer = new Offer($"hotel", "destination", DateOnly.FromDayNumber(4), 7, "city", 1300, "url", "website");

        Assert.Throws<InvalidOperationException>(() =>
        {
            DoWithTransactionAndRollback(database =>
            {
                var repository = new OffersInMemoryRepository(database);

                repository.Add(offer);
                repository.Add(offer);
            });
        });
    }

    [Test]
    public async Task modifying_price_should_succeed()
    {
        var offer = new Offer($"hotel", "destination", DateOnly.FromDayNumber(4), 7, "city", 1300, "url", "website");

        var pricesList = new List<int>();
        
        await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);

            repository.Add(offer);

            var getOffer = await repository.Get(offer.Id);
            getOffer.IfSome(o => pricesList.Add(o.Price));

            repository.ModifyPrice(offer.Id, 1400);

            getOffer = await repository.Get(offer.Id);
            getOffer.IfSome(o => pricesList.Add(o.Price));

            repository.ModifyPrice(offer.Id, 1100);

            getOffer = await repository.Get(offer.Id);
            getOffer.IfSome(o => pricesList.Add(o.Price));
        });
        
        CollectionAssert.AreEqual(
            new[] { 1300, 1400, 1100 },
            pricesList);
    }

    [Test]
    public void modifying_price_of_unknown_offer_should_throw_exception()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            DoWithTransactionAndRollback(database =>
            {
                var repository = new OffersInMemoryRepository(database);
            
                repository.ModifyPrice(Guid.NewGuid(), 1400);
            });
        });
    }

    [Test]
    public void removing_unknown_offer_should_throw_exception()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            DoWithTransactionAndRollback(database =>
            {
                var repository = new OffersInMemoryRepository(database);

                repository.Remove(Guid.NewGuid());
            });
        });
    }

    [Test]
    public async Task last_departure_date_should_be_none_value()
    {
        var departureDate = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);
        
            var date = await repository.GetLastDepartureDate("website");
            return date;
        });

        Assert.That(departureDate.IsNone, Is.True);
    }

    [Test]
    public async Task last_departure_date_should_be_the_latest_day()
    {
        var offerOne = new Offer("hotel", "destination", DateOnly.FromDayNumber(5), 4, "city", 1200, "url", "website");
        var offerTwo = new Offer("hotel", "destination", DateOnly.FromDayNumber(6), 4, "city", 1200, "url", "website");
        var offerThree = new Offer("hotel", "destination", DateOnly.FromDayNumber(4), 4, "city", 1200, "url", "website");

        var departureDate = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);

            repository.Add(offerOne);
            repository.Add(offerTwo);
            repository.Add(offerThree);

            var date = await repository.GetLastDepartureDate("website");
            return date;
        });
        
        Assert.That(departureDate.IsNone, Is.False);
        Assert.That(departureDate.Data.DayNumber, Is.EqualTo(6));
    }

    [Test]
    public async Task add_remove_add_with_another_price_check_if_new_price_was_saved()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(5), 4, "city", 1200, "url", "website");
        var sameOfferWithAnotherPrice = new Offer("hotel", "destination", DateOnly.FromDayNumber(5), 4, "city", 1500, "url", "website");

        var getOffer = await DoWithTransactionAndRollback(async database =>
        {
            var repository = new OffersInMemoryRepository(database);

            repository.Add(offer);
            repository.Remove(offer.Id);
            repository.Add(sameOfferWithAnotherPrice);

            var o = await repository.Get(offer.Id);
            return o;
        });
        
        Assert.That(getOffer.Data.Price, Is.EqualTo(1500));
    }
    
    [Test]
    public async Task add_offer_with_some_website_name_and_last_departure_date_for_another_should_be_none()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(5), 4, "city", 1200, "url", "website");
        
        var departureDate = await DoWithTransactionAndRollback(async connection =>
        {
            var repository = new OffersInMemoryRepository(connection);

            repository.Add(offer);
            
            var date = await repository.GetLastDepartureDate("another-website");
            return date;
        });
        
        Assert.That(departureDate.IsNone, Is.True);
    }
    
    [Test]
    public async Task add_offers_with_different_website_names_check_if_read_correct_ones()
    {
        var offerOne = new Offer("hotel-1", "destination", DateOnly.FromDayNumber(5), 4, "city", 1200, "url", "website");
        var offerTwo = new Offer("hotel-2", "destination", DateOnly.FromDayNumber(5), 4, "city", 1200, "url", "website-x");
        var offerThree = new Offer("hotel-3", "destination", DateOnly.FromDayNumber(5), 4, "city", 1200, "url", "website-y");
        var offerFour = new Offer("hotel-4", "destination", DateOnly.FromDayNumber(5), 4, "city", 1200, "url", "website");
        
        var getOffers = await DoWithTransactionAndRollback(async connection =>
        {
            var repository = new OffersInMemoryRepository(connection);

            repository.Add(offerOne);
            repository.Add(offerTwo);
            repository.Add(offerThree);
            repository.Add(offerFour);

            var offers = await repository.GetAllByWebsiteName("website");
            return offers;
        });
        
        Assert.That(getOffers.Elements, Has.Count.EqualTo(2));
        CollectionAssert.AreEquivalent(
            new[] { "hotel-1", "hotel-4" },
            getOffers.Elements.Select(o => o.Hotel));
    }
    
    [Test]
    public async Task add_offers_with_different_website_names_delete_it_and_check_if_read_correct_ones()
    {
        var offerOne = new Offer("hotel-1", "destination", DateOnly.FromDayNumber(5), 4, "city", 1200, "url", "website");
        var offerTwo = new Offer("hotel-2", "destination", DateOnly.FromDayNumber(5), 4, "city", 1200, "url", "website");
        
        var (getOffers, getRemovedOffers) = await DoWithTransactionAndRollback(async (connection) =>
        {
            var repository = new OffersInMemoryRepository(connection);

            repository.Add(offerOne);
            repository.Add(offerTwo);

            repository.Remove(offerOne.Id);

            var offers = await repository.GetAllByWebsiteName("website");
            var removedOffers = await repository.GetAllRemovedByWebsiteName("website");
            return (offers, removedOffers);
        });
        
        Assert.That(getOffers.Elements, Has.Count.EqualTo(1));
        Assert.That(getOffers.Elements.First().Hotel, Is.EqualTo("hotel-2"));
        
        Assert.That(getRemovedOffers.Elements, Has.Count.EqualTo(1));
        Assert.That(getRemovedOffers.Elements.First().Hotel, Is.EqualTo("hotel-1"));
    }
}
