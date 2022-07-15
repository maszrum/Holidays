using System.Text;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.Monads;
using Holidays.Core.OfferModel;
using Holidays.Selenium;
using OpenQA.Selenium;

namespace Holidays.DataSource.Tui;

// ReSharper disable once UnusedType.Global
public class TuiOffersDataSource : IOffersDataSource
{
    private readonly OffersDataSourceSettings _settings;
    private readonly WebDriverFactory _webDriverFactory;

    public TuiOffersDataSource(
        OffersDataSourceSettings settings, 
        WebDriverFactory webDriverFactory)
    {
        _settings = settings;
        _webDriverFactory = webDriverFactory;
    }

    public string WebsiteName => Constants.WebsiteName;
    
    public async Task<Result<Offers>> GetOffers(DateOnly maxDepartureDate)
    {
        try
        {
            var offers = await GetOffersOrThrow(maxDepartureDate);
            return Result.Success(offers);
        }
        catch (WebDriverException webDriverException)
        {
            return new WebScrapingError(webDriverException, Constants.WebsiteName);
        }
        catch (TimeoutException)
        {
            return new TimeoutError(Constants.WebsiteName);
        }
    }

    private async Task<Offers> GetOffersOrThrow(DateOnly maxDepartureDate)
    {
        using var webDriver = _webDriverFactory.Create();

        await Task.Delay(1_000);

        await OpenStartUrlAndCloseCookiePopup(webDriver);
        
        var collector = new OfferElementsCollector(webDriver);
        var extractor = new OfferDataExtractor();
        
        Offer? lastOffer;
        var offers = Enumerable.Empty<Offer>();

        do
        {
            var timeout = TimeSpan.FromSeconds(_settings.CollectingTimeoutSeconds);
            var collectedElements = await collector.Collect(timeout);

            var collectedOffers = collectedElements
                .Select(element => extractor.Extract(element))
                .ToArray();
            
            offers = offers.Concat(collectedOffers.Where(offer => offer.DepartureDate <= maxDepartureDate));
    
            lastOffer = collectedOffers.LastOrDefault();
        } while (lastOffer is not null && lastOffer.DepartureDate < maxDepartureDate);

        return new Offers(offers);
    }

    private static async Task OpenStartUrlAndCloseCookiePopup(IWebDriver webDriver)
    {
        webDriver
            .Navigate()
            .GoToUrl(GetUrl());

        await Task.Delay(1_000);

        if (webDriver.TryFindElement(By.ClassName("cookies-bar__button--accept"), out var closeCookiesElement))
        {
            closeCookiesElement.Click();
        }
    }

    private static string GetUrl()
    {
        var queryParams = new[]
        {
            "",
            "flightDate",
            "byPlane",
            "T",
            "dF",
            "6",
            "dT",
            "14",
            "ctAdult",
            "2",
            "ctChild",
            "0",
            "board",
            "GT06-AI%2520GT06-FBP",
            "minHotelCategory",
            "defaultHotelCategory",
            "tripAdvisorRating",
            "defaultTripAdvisorRating",
            "beach_distance",
            "defaultBeachDistance",
            "tripType",
            "WS"
        };

        var query = string.Join("%3A", queryParams);

        return new StringBuilder()
            .Append("https://www.tui.pl/all-inclusive")
            .Append("?pm_source=MENU")
            .Append("&pm_name=All_Inclusive")
            .Append("&q=")
            .Append(query)
            .Append("&fullPrice=false")
            .ToString();
    }
}
