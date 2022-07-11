using Holidays.Core.Algorithms.ChangesDetection;
using Holidays.Core.Eventing;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.OfferModel;
using Holidays.Eventing;
using Serilog;

namespace Holidays.Cli;

internal class ChangesDetectionJob
{
    private readonly EventBus _eventBus;
    private readonly IOffersRepository _offersRepository;
    private readonly ILogger _logger;

    public ChangesDetectionJob(
        EventBus eventBus,
        IOffersRepository offersRepository, 
        ILogger logger)
    {
        _eventBus = eventBus;
        _offersRepository = offersRepository;
        _logger = logger;
    }

    public async Task Run(IOffersDataSource dataSource, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var previousState = await _offersRepository.GetAll();

            _logger.Debug(
                "Starting scraping with data source: {ScraperType}", 
                dataSource.GetType().Name);
            
            var currentState = await GetOffers(dataSource);
            
            _logger.Debug("Finished scraping");
            
            if (currentState.IsError)
            {
                _logger.Error(currentState.Error);
                continue;
            }

            var changes = new OfferChangesDetector()
                .DetectChanges(previousState, currentState.Data);
            
            _logger.Information(
                "Offers scraped: {OffersScraped}, changes detected: {DetectedChanges}", 
                currentState.Data.Elements.Count, 
                changes.Count);

            var events = changes.Select(GetEventBasedOnChange);

            foreach (var @event in events)
            {
                await _eventBus.Publish(@event, cancellationToken);
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken); // TODO: add configuration key
        }
    }

    private static IEvent GetEventBasedOnChange(DetectedChange change)
    {
        return change.ChangeType switch
        {
            OfferChangeType.PriceChanged => new OfferPriceChanged(change.Offer, change.OfferBeforeChange.Price, DateTime.UtcNow),
            OfferChangeType.OfferAdded => OfferAdded.ForNewOffer(change.Offer),
            OfferChangeType.OfferRemoved => new OfferRemoved(change.Offer.Id, DateTime.UtcNow),
            _ => throw new ArgumentOutOfRangeException(nameof(change.ChangeType), change.ChangeType.ToString())
        };
    }

    private static async Task<Result<Offers>> GetOffers(IOffersDataSource dataSource)
    {
        bool OffersFilter(Offer offer)
        {
            return DateOnly.FromDateTime(DateTime.Now.AddDays(1)) >= offer.DepartureDate;
        }

        var offers = await dataSource.GetOffers(OffersFilter);
        return offers;
    }
}
