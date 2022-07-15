using Holidays.Cli;
using Holidays.Core.OfferModel;
using Holidays.Eventing;
using Holidays.InMemoryStore;
using Holidays.Postgres;
using Holidays.Postgres.Initialization;
using Holidays.Selenium;

await ApplicationBootstrapper.Run(async app =>
{
    var offersDataSourceSettings = app.Configuration.Get<OffersDataSourceSettings>();

    app.GetLogger().Information(
        "Starting application with {DataSourceType} data source",
        offersDataSourceSettings.Provider);

    var webDriverFactory = new WebDriverFactory(app.Configuration.Get<SeleniumSettings>());
    var offersDataSource = new DataSourceFactory(offersDataSourceSettings, webDriverFactory)
        .Get(offersDataSourceSettings.Provider);

    var postgresConnectionFactory = new PostgresConnectionFactory(app.Configuration.Get<PostgresSettings>());

    var databaseInitializer = new DatabaseInitializer(postgresConnectionFactory);
    await databaseInitializer.InitializeIfNeed();

    Offers persistedActiveOffers;

    await using (var connection = await postgresConnectionFactory.CreateConnection())
    await using (var postgresOffersRepository = new OffersPostgresRepository(connection))
    {
        persistedActiveOffers = await postgresOffersRepository.GetAllByWebsiteName(offersDataSource.WebsiteName);
    }

    var inMemoryStore = InMemoryDatabase.CreateWithInitialState(persistedActiveOffers);
    var offersInMemoryRepository = new OffersInMemoryRepository(inMemoryStore);

    var eventBus = await new EventBusBuilder()
        .ConfigureRabbitMqIfTurnedOn(app)
        .RegisterEventHandlers(inMemoryStore, postgresConnectionFactory)
        .Build();

    var job = new ChangesDetectionJob(
        offersDataSourceSettings,
        eventBus,
        offersInMemoryRepository,
        app.GetLogger<ChangesDetectionJob>());

    app.GetLogger().Information("Application started");

    try
    {
        await job.Run(offersDataSource, app.ApplicationCancellationToken);
    }
    finally
    {
        app.GetLogger().Information("Application closed");
    }
});
