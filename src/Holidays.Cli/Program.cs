using Holidays.Cli;
using Holidays.Configuration;
using Holidays.Core.OfferModel;
using Holidays.DataSource.Rainbow;
using Holidays.Eventing;
using Holidays.InMemoryStore;
using Holidays.InMemoryStore.EventHandlers;
using Holidays.Postgres;
using Holidays.Postgres.EventHandlers;
using Holidays.Postgres.Initialization;
using Holidays.Selenium;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var configuration = new ApplicationConfiguration(
    "appsettings.json", 
    overrideWithEnvironmentVariables: false);

var webDriverFactory = new WebDriverFactory(configuration.Get<SeleniumSettings>());
var offersDataSource = new RainbowOffersDataSource(webDriverFactory);

var postgresConnectionFactory = new PostgresConnectionFactory(configuration.Get<PostgresSettings>());

var databaseInitializer = new DatabaseInitializer(postgresConnectionFactory);
await databaseInitializer.InitializeIfNeed();

Offers persistedActiveOffers, persistedRemovedOffers;

await using(var connection = await postgresConnectionFactory.CreateConnection())
await using (var postgresOffersRepository = new OffersPostgresRepository(connection))
{
    persistedActiveOffers = await postgresOffersRepository.GetAllByWebsiteName(offersDataSource.WebsiteName);
    persistedRemovedOffers = await postgresOffersRepository.GetAllRemovedByWebsiteName(offersDataSource.WebsiteName);
}

var inMemoryStore = InMemoryDatabase.CreateWithInitialState(persistedActiveOffers, persistedRemovedOffers);

var eventBusBuilder = new EventBusBuilder();

eventBusBuilder
    .ForEventType<OfferAdded>()
    .RegisterHandler(() => new OfferAddedInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandler(() => new OfferAddedPostgresEventHandler(postgresConnectionFactory));

eventBusBuilder
    .ForEventType<OfferRemoved>()
    .RegisterHandler(() => new OfferRemovedInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandler(() => new OfferRemovedPostgresEventHandler(postgresConnectionFactory));

eventBusBuilder
    .ForEventType<OfferPriceChanged>()
    .RegisterHandler(() => new OfferPriceChangedInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandler(() => new OfferPriceChangedPostgresEventHandler(postgresConnectionFactory));

eventBusBuilder
    .ForEventType<OfferStartedTracking>()
    .RegisterHandler(() => new OfferStartedTrackingInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandler(() => new OfferStartedTrackingPostgresEventHandler(postgresConnectionFactory));

var eventBus = eventBusBuilder.Build();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, _) => cts.Cancel();

var offersInMemoryRepository = new OffersInMemoryRepository(inMemoryStore);
var job = new ChangesDetectionJob(eventBus, offersInMemoryRepository, logger.ForContext<ChangesDetectionJob>());

await job.Run(offersDataSource, cts.Token);

Console.WriteLine("Finished.");
