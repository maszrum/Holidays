using Holidays.Core.Algorithms.ChangesDetection;
using Holidays.Core.Events.OfferModel;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.OfferModel;
using Holidays.Eventing;
using Holidays.Eventing.Core;
using Holidays.Selenium;
using Serilog;

namespace Holidays.CollectingApp;

internal class ChangesDetectionJob
{
    private readonly OffersDataSourceSettings _settings;
    private readonly IEventBus _eventBus;
    private readonly IOffersRepository _offersRepository;
    private readonly ILogger _logger;

    public ChangesDetectionJob(
        OffersDataSourceSettings settings,
        IEventBus eventBus,
        IOffersRepository offersRepository,
        ILogger logger)
    {
        _settings = settings;
        _eventBus = eventBus;
        _offersRepository = offersRepository;
        _logger = logger;
    }

    public async Task Run(IOffersDataSource dataSource, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var previousState = await _offersRepository.GetAllByWebsiteName(dataSource.WebsiteName);
            var lastDepartureDay = (await _offersRepository.GetLastDepartureDate(dataSource.WebsiteName))
                .WithDefaultValue(DateOnly.FromDayNumber(0));

            _logger.Debug(
                "Starting scraping with data source: {ScraperType}",
                dataSource.GetType().Name);

            var currentState = await GetOffers(dataSource);

            if (currentState.IsError)
            {
                _logger.Error(currentState.Error);
                await Task.Delay(5_000, cancellationToken);
                continue;
            }

            _logger.Debug("Finished scraping");

            var changes = new OfferChangesDetector()
                .DetectChanges(previousState, currentState.Data);

            _logger.Information(
                "Offers scraped: {OffersScraped}, changes detected: {DetectedChanges}",
                currentState.Data.Elements.Count,
                changes.Count);

            var events = changes.Select(change => GetEventBasedOnChange(change, lastDepartureDay));

            foreach (var @event in events)
            {
                await _eventBus.Publish(@event, cancellationToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(_settings.PauseBetweenCollectionsSeconds), cancellationToken);
        }
    }

    private static IEvent GetEventBasedOnChange(DetectedChange change, DateOnly lastDepartureDay)
    {
        var startedTracking = change.Offer.DepartureDate > lastDepartureDay;
        var offer = change.Offer;

        return (change.ChangeType, startedTracking) switch
        {
            (OfferChangeType.OfferAdded, true) => offer.ToStartedTrackingEvent(DateTime.UtcNow),
            (OfferChangeType.OfferAdded, false) => offer.ToOfferAddedEvent(DateTime.UtcNow),
            (OfferChangeType.PriceChanged, _) => new OfferPriceChanged(offer.Id, offer.Price, change.OfferBeforeChange.Price, DateTime.UtcNow),
            (OfferChangeType.OfferRemoved, _) => new OfferRemoved(offer.Id, DateTime.UtcNow),
            _ => throw new ArgumentOutOfRangeException(nameof(change.ChangeType), change.ChangeType.ToString())
        };
    }

    private async Task<Result<Offers>> GetOffers(IOffersDataSource dataSource)
    {
        var maxDepartureDate = DateOnly.FromDateTime(DateTime.Now.AddDays(_settings.NumberOfDaysToCollectOffers));

        var offers = await dataSource.GetOffers(maxDepartureDate);
        return offers;
    }
}
