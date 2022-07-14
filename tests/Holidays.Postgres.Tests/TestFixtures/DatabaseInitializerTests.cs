using Holidays.Core.OfferModel;
using Holidays.Postgres.Initialization;
using NUnit.Framework;

namespace Holidays.Postgres.Tests.TestFixtures;

[TestFixture]
public class DatabaseInitializerTests : DatabaseTestsBase
{
    [Test]
    public async Task initialize_forcefully_should_remove_offer_table_and_create_again()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(12), 8, "city", 1200, "url", "website");

        int offersCountBeforeInitialization, offersCountAfterInitialization;
        
        await using (var connection = await ConnectionFactory.CreateConnection())
        {
            var offersRepository = new OffersPostgresRepository(connection);
            await offersRepository.Add(offer);

            offersCountBeforeInitialization = await offersRepository.Count() + await offersRepository.CountRemoved();
        }

        var initializer = new DatabaseInitializer(ConnectionFactory);

        await initializer.InitializeForcefully();
        
        await using (var connection = await ConnectionFactory.CreateConnection())
        {
            var offersRepository = new OffersPostgresRepository(connection);

            offersCountAfterInitialization = await offersRepository.Count() + await offersRepository.CountRemoved();
        }
        
        Assert.That(offersCountBeforeInitialization, Is.EqualTo(1));
        Assert.That(offersCountAfterInitialization, Is.EqualTo(0));
    }
    
    [Test]
    public async Task initialize_if_need_should_not_remove_offer_table()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(12), 8, "city", 1200, "url", "website");

        int offersCountBeforeInitialization, offersCountAfterInitialization;
        
        await using (var connection = await ConnectionFactory.CreateConnection())
        {
            var offersRepository = new OffersPostgresRepository(connection);
            await offersRepository.Add(offer);

            offersCountBeforeInitialization = await offersRepository.Count() + await offersRepository.CountRemoved();
        }

        var initializer = new DatabaseInitializer(ConnectionFactory);

        await initializer.InitializeIfNeed();
        
        await using (var connection = await ConnectionFactory.CreateConnection())
        {
            var offersRepository = new OffersPostgresRepository(connection);

            offersCountAfterInitialization = await offersRepository.Count() + await offersRepository.CountRemoved();
        }
        
        Assert.That(offersCountBeforeInitialization, Is.EqualTo(1));
        Assert.That(offersCountAfterInitialization, Is.EqualTo(1));
        
        MarkDatabaseAsDirty();
    }
    
    [Test]
    public async Task initialize_forcefully_should_remove_offer_event_log_table_and_create_again()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(12), 8, "city", 1200, "url", "website");
        var @event = new OfferAdded(offer, offer.Id, DateTime.UtcNow);

        int offerEventLogCountBeforeInitialization, offerEventLogCountAfterInitialization;
        
        await using (var connection = await ConnectionFactory.CreateConnection())
        {
            var offersRepository = new OffersPostgresRepository(connection);
            await offersRepository.Add(offer);

            var offerEventLogRepository = new OfferEventLogPostgresRepository(connection);
            await offerEventLogRepository.Add(@event);

            offerEventLogCountBeforeInitialization = await offerEventLogRepository.Count();
        }

        var initializer = new DatabaseInitializer(ConnectionFactory);

        await initializer.InitializeForcefully();
        
        await using (var connection = await ConnectionFactory.CreateConnection())
        {
            var offerEventLogRepository = new OfferEventLogPostgresRepository(connection);

            offerEventLogCountAfterInitialization = await offerEventLogRepository.Count();
        }
        
        Assert.That(offerEventLogCountBeforeInitialization, Is.EqualTo(1));
        Assert.That(offerEventLogCountAfterInitialization, Is.EqualTo(0));
    }
    
    [Test]
    public async Task initialize_if_need_should_not_remove_offer_event_log_table()
    {
        var offer = new Offer("hotel", "destination", DateOnly.FromDayNumber(12), 8, "city", 1200, "url", "website");
        var @event = new OfferAdded(offer, offer.Id, DateTime.UtcNow);

        int offerEventLogCountBeforeInitialization, offerEventLogCountAfterInitialization;
        
        await using (var connection = await ConnectionFactory.CreateConnection())
        {
            var offersRepository = new OffersPostgresRepository(connection);
            await offersRepository.Add(offer);

            var offerEventLogRepository = new OfferEventLogPostgresRepository(connection);
            await offerEventLogRepository.Add(@event);

            offerEventLogCountBeforeInitialization = await offerEventLogRepository.Count();
        }

        var initializer = new DatabaseInitializer(ConnectionFactory);

        await initializer.InitializeIfNeed();
        
        await using (var connection = await ConnectionFactory.CreateConnection())
        {
            var offerEventLogRepository = new OfferEventLogPostgresRepository(connection);

            offerEventLogCountAfterInitialization = await offerEventLogRepository.Count();
        }
        
        Assert.That(offerEventLogCountBeforeInitialization, Is.EqualTo(1));
        Assert.That(offerEventLogCountAfterInitialization, Is.EqualTo(1));
        
        MarkDatabaseAsDirty();
    }
}
