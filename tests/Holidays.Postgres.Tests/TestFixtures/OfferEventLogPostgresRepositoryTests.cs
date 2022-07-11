using Holidays.Core.OfferModel;
using NUnit.Framework;

namespace Holidays.Postgres.Tests.TestFixtures;

[TestFixture]
public class OfferEventLogPostgresRepositoryTests : DatabaseTestsBase
{
    [Test]
    public async Task count_offer_event_logs_should_return_zero()
    {
        var count = await DoWithTransactionAndRollback(async (connection, transaction) =>
        {
            var eventLogRepository = new OfferEventLogPostgresRepository(connection, transaction);
            return await eventLogRepository.Count();
        });
        
        Assert.That(count, Is.EqualTo(0));
    }
    
    [Test]
    public async Task count_offer_event_logs_should_return_three()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(3), 6, "city", 1800, "url");
        var eventOne = new OfferAdded(offer.Id);
        var eventTwo = new OfferPriceChanged(offer.Id, offer.Price, 1400);
        var eventThree = new OfferRemoved(offer.Id);
        
        var count = await DoWithTransactionAndRollback(async (connection, transaction) =>
        {
            var offersRepository = new OffersPostgresRepository(connection, transaction);
            var eventLogRepository = new OfferEventLogPostgresRepository(connection, transaction);
            
            await offersRepository.Add(offer);

            await eventLogRepository.Add(eventOne);
            await eventLogRepository.Add(eventTwo);
            await eventLogRepository.Add(eventThree);

            var eventsCount = await eventLogRepository.Count();
            return eventsCount;
        });
        
        Assert.That(count, Is.EqualTo(3));
    }
    
    [Test]
    public async Task add_three_different_event_logs_and_get_it_by_offer_id()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(3), 6, "city", 1800, "url");
        var eventOne = new OfferAdded(offer.Id);
        var eventTwo = new OfferPriceChanged(offer.Id, offer.Price, 1400);
        var eventThree = new OfferRemoved(offer.Id);

        var getEvents = await DoWithTransactionAndRollback(async (connection, transaction) =>
        {
            var offersRepository = new OffersPostgresRepository(connection, transaction);
            var eventLogRepository = new OfferEventLogPostgresRepository(connection, transaction);
            
            await offersRepository.Add(offer);

            await eventLogRepository.Add(eventOne);
            await eventLogRepository.Add(eventTwo);
            await eventLogRepository.Add(eventThree);

            var events = await eventLogRepository.GetByOfferId(offer.Id);
            return events;
        });
        
        Assert.That(getEvents, Has.Count.EqualTo(3));
        // TODO: check order
    }
    
    [Test]
    public async Task add_three_events_for_two_offers_and_check_count_result()
    {
        var offerOne = new Offer("hotel-1", "destination", DateOnly.FromDayNumber(3), 6, "city", 1800, "url");
        var offerTwo = new Offer("hotel-2", "destination", DateOnly.FromDayNumber(3), 6, "city", 1800, "url");
        var eventOne = new OfferAdded(offerOne.Id);
        var eventTwo = new OfferPriceChanged(offerOne.Id, offerOne.Price, 1400);
        var eventThree = new OfferAdded(offerTwo.Id);

        var (eventCountOne, eventCountTwo) = await DoWithTransactionAndRollback(async (connection, transaction) =>
        {
            var offersRepository = new OffersPostgresRepository(connection, transaction);
            var eventLogRepository = new OfferEventLogPostgresRepository(connection, transaction);
            
            await offersRepository.Add(offerOne);
            await offersRepository.Add(offerTwo);

            await eventLogRepository.Add(eventOne);
            await eventLogRepository.Add(eventTwo);
            await eventLogRepository.Add(eventThree);

            var eventsOne = await eventLogRepository.GetByOfferId(offerOne.Id);
            var eventsTwo = await eventLogRepository.GetByOfferId(offerTwo.Id);
            return (eventsOne.Count, eventsTwo.Count);
        });
        
        Assert.That(eventCountOne, Is.EqualTo(2));
        Assert.That(eventCountTwo, Is.EqualTo(1));
    }
}
