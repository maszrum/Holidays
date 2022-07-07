using System.Text;
using Holidays.Core.Monads;
using Holidays.Core.OfferModel;
using Holidays.Core.InfrastructureInterfaces;
using Holidays.Selenium;
using OpenQA.Selenium;

namespace Holidays.DataSource.Rainbow;

public class RainbowOffersDataSource : IOffersDataSource
{
    private const string WebsiteName = "Rainbow";
    
    private readonly WebDriverFactory _webDriverFactory;

    public RainbowOffersDataSource(WebDriverFactory webDriverFactory)
    {
        _webDriverFactory = webDriverFactory;
    }

    public async Task<Result<Offers>> GetOffers(Predicate<Offer> predicate)
    {
        try
        {
            var offers = await GetOffersOrThrow(predicate);
            return Result.Success(offers);
        }
        catch (WebDriverException webDriverException)
        {
            return new WebScrapingError(webDriverException, WebsiteName);
        }
        catch (TimeoutException)
        {
            return new TimeoutError(WebsiteName);
        }
    }
    
    private async Task<Offers> GetOffersOrThrow(Predicate<Offer> predicate)
    {
        using var webDriver = _webDriverFactory.Create();

        await OpenStartUrlAndCloseCookiePopup(webDriver);
        
        var collector = new OfferElementsCollector(webDriver);
        var extractor = new OfferDataExtractor();
        
        Offer lastOffer;
        var offers = Enumerable.Empty<Offer>();

        do
        {
            var collectedElements = await collector.Collect(TimeSpan.FromMinutes(1)); // TODO: move timeout to configuration

            var collectedOffers = collectedElements
                .Select(element => extractor.Extract(element))
                .ToArray();
            
            offers = offers.Concat(collectedOffers.Where(offer => predicate(offer)));
    
            lastOffer = collectedOffers.Last();
        } while (predicate(lastOffer));

        return new Offers(offers);
    }

    private static async Task OpenStartUrlAndCloseCookiePopup(IWebDriver webDriver)
    {
        webDriver
            .Navigate()
            .GoToUrl(GetUrl());

        await Task.Delay(1);

        if (webDriver.TryFindElement(By.ClassName("rodo-alert__close"), out var closeCookiesElement))
        {
            closeCookiesElement.Click();
        }
    }

    private static string GetUrl()
    {
        return new StringBuilder()
            .Append("https://r.pl/all-inclusive")
            .Append("?liczbaPokoi=1")
            .Append("&sortowanie=termin-asc")
            .Append("&widok=bloczki")
            .Append("&cena[]=avg")
            .Append("&dlugoscPobytu[]=7-9")
            .Append("&typTransportu[]=AIR")
            .Append("&typTransportu[]=dreamliner")
            .Append("&wyzywienia[]=all-inclusive")
            .ToString();
    }
}
