using Holidays.Cli;
using Holidays.Configuration;
using Holidays.Core.OfferModel;
using Holidays.Eventing;
using Holidays.Eventing.RabbitMq;
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

var offersDataSourceSettings = configuration.Get<OffersDatasourceSettings>();

var webDriverFactory = new WebDriverFactory(configuration.Get<SeleniumSettings>());
var offersDataSource = new DataSourceFactory(webDriverFactory).Get(offersDataSourceSettings.Provider);

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

var rabbitMqSettings = configuration.Get<RabbitMqSettings>();
if (rabbitMqSettings.UseRabbitMq)
{
    eventBusBuilder.UseRabbitMq(rabbitMqSettings, options =>
    {
        options.AddEventTypesAssembly<OfferAdded>();
        options.OnUnknownEventType(eventTypeName =>
        {
            logger.ForContext<RabbitMqProvider>().Error(
                "Received event with unknown event type: {EventType}", 
                eventTypeName);
            
            return Task.CompletedTask;
        });
        options.OnDeserializationError((exception, eventTypeName) =>
        {
            logger.ForContext<RabbitMqProvider>().Error(
                exception, 
                "An error occured on deserializing event data {EventType}", 
                eventTypeName);
            
            return Task.CompletedTask;
        });
    });
}

eventBusBuilder
    .ForEventType<OfferAdded>()
    .RegisterHandlerForAllEvents(() => new OfferAddedInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandlerForLocalEvents(() => new OfferAddedPostgresEventHandler(postgresConnectionFactory));

eventBusBuilder
    .ForEventType<OfferRemoved>()
    .RegisterHandlerForAllEvents(() => new OfferRemovedInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandlerForLocalEvents(() => new OfferRemovedPostgresEventHandler(postgresConnectionFactory));

eventBusBuilder
    .ForEventType<OfferPriceChanged>()
    .RegisterHandlerForAllEvents(() => new OfferPriceChangedInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandlerForLocalEvents(() => new OfferPriceChangedPostgresEventHandler(postgresConnectionFactory));

eventBusBuilder
    .ForEventType<OfferStartedTracking>()
    .RegisterHandlerForAllEvents(() => new OfferStartedTrackingInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandlerForLocalEvents(() => new OfferStartedTrackingPostgresEventHandler(postgresConnectionFactory));

await using var eventBus = await eventBusBuilder.Build();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, _) => cts.Cancel();

var offersInMemoryRepository = new OffersInMemoryRepository(inMemoryStore);
var job = new ChangesDetectionJob(eventBus, offersInMemoryRepository, logger.ForContext<ChangesDetectionJob>());

await job.Run(offersDataSource, cts.Token);

Console.WriteLine("Finished.");
