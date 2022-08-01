using System.Text;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Core.Monads;
using Holidays.Core.OfferModel;
using Holidays.Selenium;
using OpenQA.Selenium;

namespace Holidays.DataSource.Itaka;

// ReSharper disable once UnusedType.Global

public class ItakaOffersDataSource : IOffersDataSource
{
    private readonly OffersDataSourceSettings _settings;
    private readonly WebDriverFactory _webDriverFactory;

    public ItakaOffersDataSource(
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

        var timeout = TimeSpan.FromSeconds(_settings.CollectingTimeoutSeconds);

        do
        {
            var collectedElements = await collector.Collect(timeout);

            var collectedOffers = collectedElements
                .Select(element => extractor.Extract(element))
                .ToArray();

            offers = offers.Concat(collectedOffers.Where(offer => offer.DepartureDate <= maxDepartureDate));

            lastOffer = collectedOffers.LastOrDefault();
        } while (lastOffer is not null && lastOffer.DepartureDate <= maxDepartureDate);

        return new Offers(offers);
    }

    private static async Task OpenStartUrlAndCloseCookiePopup(IWebDriver webDriver)
    {
        webDriver
            .Navigate()
            .GoToUrl(GetUrl());

        await Task.Delay(1_000);

        if (webDriver.TryFindElement(By.CssSelector(".cookie-info button"), out var closeCookiesElement))
        {
            closeCookiesElement.Click();
        }
    }

    private static string GetUrl()
    {
        var dateFrom = DateTime.Now.ToString("yyyy-MM-dd");

        return new StringBuilder()
            .Append("https://www.itaka.pl/all-inclusive/")
            .Append("?view=offerList")
            .Append("&package-type=wczasy")
            .Append("&adults=2")
            .Append("&date-from=").Append(dateFrom)
            .Append("&food=allInclusive")
            .Append("&order=dateFromAsc")
            .Append("&total-price=0")
            .Append("&page=1")
            .Append("&transport=flight")
            .Append("&currency=PLN")
            .ToString();
    }
}
