using Holidays.Cli;
using Holidays.Configuration;
using Holidays.Core.OfferModel;
using Holidays.DataSource.Rainbow;
using Holidays.Eventing;
using Holidays.InMemoryStore;
using Holidays.InMemoryStore.EventHandlers;
using Holidays.MongoDb;
using Holidays.MongoDb.EventHandlers;
using Holidays.Selenium;
using Serilog;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var configuration = new ApplicationConfiguration(
    "appsettings.json", 
    overrideWithEnvironmentVariables: false);

var mongoDbConnectionFactory = new ConnectionFactory(configuration.Get<MongoDbSettings>());
var offersMongoRepository = new OffersMongoRepository(mongoDbConnectionFactory);
var persistedActiveOffers = await offersMongoRepository.GetAll();
var persistedRemovedOffers = await offersMongoRepository.GetAllRemoved();

var inMemoryStore = InMemoryStore.CreateWithInitialState(persistedActiveOffers, persistedRemovedOffers);

var eventBusBuilder = new EventBusBuilder();

eventBusBuilder
    .ForEventType<OfferAdded>()
    .RegisterHandler(() => new OfferAddedInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandler(() => new OfferAddedMongoDbEventHandler(mongoDbConnectionFactory));

eventBusBuilder
    .ForEventType<OfferRemoved>()
    .RegisterHandler(() => new OfferRemovedInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandler(() => new OfferRemovedMongoDbEventHandler(mongoDbConnectionFactory));

eventBusBuilder
    .ForEventType<OfferPriceChanged>()
    .RegisterHandler(() => new OfferPriceChangedInMemoryStoreEventHandler(inMemoryStore))
    .RegisterHandler(() => new OfferPriceChangedMongoDbEventHandler(mongoDbConnectionFactory));

var eventBus = eventBusBuilder.Build();

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, _) => cts.Cancel();

var offersInMemoryRepository = new OffersInMemoryRepository(inMemoryStore);
var job = new ChangesDetectionJob(eventBus, offersInMemoryRepository, logger.ForContext<ChangesDetectionJob>());

var webDriverFactory = new WebDriverFactory(configuration.Get<SeleniumSettings>());
var offersDataSource = new RainbowOffersDataSource(webDriverFactory);

await job.Run(offersDataSource, cts.Token);

Console.WriteLine("Finished.");
