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
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(3), 6, "city", 1800, "url", "website");
        var eventOne = new OfferAdded(offer.Id, DateTime.UtcNow);
        var eventTwo = new OfferPriceChanged(offer.Id, offer.Price, 1400, DateTime.UtcNow);
        var eventThree = new OfferRemoved(offer.Id, DateTime.UtcNow);
        
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
        var dateTime = new DateTime(2022, 7, 11, 17, 37, 10);
        
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(3), 6, "city", 1800, "url", "website");
        var eventOne = new OfferAdded(offer.Id, dateTime.AddSeconds(1));
        var eventTwo = new OfferPriceChanged(offer.Id, offer.Price, 1400, dateTime.AddSeconds(2));
        var eventThree = new OfferRemoved(offer.Id, dateTime.AddSeconds(3));

        var getEvents = await DoWithTransactionAndRollback(async (connection, transaction) =>
        {
            var offersRepository = new OffersPostgresRepository(connection, transaction);
            var eventLogRepository = new OfferEventLogPostgresRepository(connection, transaction);
            
            await offersRepository.Add(offer);

            // intentionally in this order
            await eventLogRepository.Add(eventTwo);
            await eventLogRepository.Add(eventOne);
            await eventLogRepository.Add(eventThree);

            var events = await eventLogRepository.GetByOfferId(offer.Id);
            return events;
        });
        
        Assert.That(getEvents, Has.Count.EqualTo(3));
        
        CollectionAssert.AreEqual(
            new[] { "OfferAdded", "OfferPriceChanged", "OfferRemoved" },
            getEvents.Select(e => e.GetType().Name));
    }
    
    [Test]
    public async Task add_three_events_for_two_offers_and_check_count_result()
    {
        var offerOne = new Offer("hotel-1", "destination", DateOnly.FromDayNumber(3), 6, "city", 1800, "url", "website");
        var offerTwo = new Offer("hotel-2", "destination", DateOnly.FromDayNumber(3), 6, "city", 1800, "url", "website");
        var eventOne = new OfferAdded(offerOne.Id, DateTime.UtcNow);
        var eventTwo = new OfferPriceChanged(offerOne.Id, offerOne.Price, 1400, DateTime.UtcNow);
        var eventThree = new OfferAdded(offerTwo.Id, DateTime.UtcNow);

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
    
    [Test]
    public async Task add_event_and_check_if_timestamp_is_correct()
    {
        var offer = new Offer("hotel-1", "destination", DateOnly.FromDayNumber(3), 6, "city", 1800, "url", "website");
        var @event = new OfferAdded(offer.Id, new DateTime(2022, 7, 11, 18, 31, 12));

        var readEvents = await DoWithTransactionAndRollback(async (connection, transaction) =>
        {
            var offersRepository = new OffersPostgresRepository(connection, transaction);
            var eventLogRepository = new OfferEventLogPostgresRepository(connection, transaction);
            
            await offersRepository.Add(offer);
            await eventLogRepository.Add(@event);

            var getEvents = await eventLogRepository.GetByOfferId(offer.Id);

            return getEvents;
        });

        Assert.That(readEvents, Has.Count.EqualTo(1));

        var readEvent = readEvents[0];
        
        Assert.That(readEvent.Timestamp, Is.EqualTo(new DateTime(2022, 7, 11, 18, 31, 12)));
    }
}
