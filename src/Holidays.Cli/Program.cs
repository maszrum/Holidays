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

var configuration = new ApplicationConfiguration(
    "appsettings.json", 
    overrideWithEnvironmentVariables: true);
    
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration.ConfigurationRoot)
    .WriteTo.Console()
    .CreateLogger();

AppDomain.CurrentDomain.UnhandledException += (_, e) =>
{
    if (e.ExceptionObject is Exception exception)
    {
        logger.Fatal(
            exception, 
            "Unhandled exception occured");
    }
    else
    {
        logger.Fatal(
            "Unhandled unknown exception occured");
    }
};

var offersDataSourceSettings = configuration.Get<OffersDatasourceSettings>();

logger.Information(
    "Starting application with {DataSourceType} data source", 
    offersDataSourceSettings.Provider);

var webDriverFactory = new WebDriverFactory(configuration.Get<SeleniumSettings>());
var offersDataSource = new DataSourceFactory(webDriverFactory).Get(offersDataSourceSettings.Provider);

var postgresConnectionFactory = new PostgresConnectionFactory(configuration.Get<PostgresSettings>());

var databaseInitializer = new DatabaseInitializer(postgresConnectionFactory);
await databaseInitializer.InitializeIfNeed();

Offers persistedActiveOffers;

await using(var connection = await postgresConnectionFactory.CreateConnection())
await using (var postgresOffersRepository = new OffersPostgresRepository(connection))
{
    persistedActiveOffers = await postgresOffersRepository.GetAllByWebsiteName(offersDataSource.WebsiteName);
}

var inMemoryStore = InMemoryDatabase.CreateWithInitialState(persistedActiveOffers);

var eventBusBuilder = new EventBusBuilder();

var rabbitMqSettings = configuration.Get<RabbitMqSettings>();
if (rabbitMqSettings.UseRabbitMq)
{
    eventBusBuilder.UseRabbitMq(rabbitMqSettings, options =>
    {
        options.OnUnknownEventType(eventTypeName =>
        {
            logger.ForContext<RabbitMqProvider>().Error(
                "Received event with unknown event type: {EventType}", 
                eventTypeName);
        });
        options.OnDeserializationError((exception, eventTypeName) =>
        {
            logger.ForContext<RabbitMqProvider>().Error(
                exception, 
                "An error occured on deserializing event data {EventType}", 
                eventTypeName);
        });
        options.OnEventReceived(@event =>
        {
            logger.ForContext<RabbitMqProvider>().Debug(
                "Received event from external broker: {EventType}", 
                @event.GetType().Name);
        });
        options.OnEventSent(@event =>
        {
            logger.ForContext<RabbitMqProvider>().Debug(
                "Sent event to external broker: {EventType}", 
                @event.GetType().Name);
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

Console.CancelKeyPress += (_, e) =>
{
    logger.Information("Closing application due to cancel key pressed");
    cts.Cancel();
    e.Cancel = true;
};

AppDomain.CurrentDomain.ProcessExit += (_, _) =>
{
    logger.Information("Closing application due to process exit event");
    cts.Cancel();
};

var offersInMemoryRepository = new OffersInMemoryRepository(inMemoryStore);
var job = new ChangesDetectionJob(eventBus, offersInMemoryRepository, logger.ForContext<ChangesDetectionJob>());

logger.Information("Application started");

try
{
    await job.Run(offersDataSource, cts.Token);
}
finally
{
    logger.Information("Application closed");
}
