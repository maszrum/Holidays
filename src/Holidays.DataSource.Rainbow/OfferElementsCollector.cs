using System.Collections.ObjectModel;
using Holidays.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Holidays.DataSource.Rainbow;

internal class OfferElementsCollector
{
    private readonly IWebDriver _driver;
    private int _collectedElementsCount;

    public OfferElementsCollector(IWebDriver driver)
    {
        _driver = driver;
    }

    public Task<IReadOnlyList<IWebElement>> Collect(TimeSpan timeout)
    {
        var loadedOffers = GetLoadedOffers();

        if (_collectedElementsCount == 0 || _collectedElementsCount != loadedOffers.Count)
        {
            var offers = loadedOffers
                .Skip(_collectedElementsCount)
                .ToArray();
            
            _collectedElementsCount = loadedOffers.Count;

            return Task.FromResult<IReadOnlyList<IWebElement>>(offers);
        }

        return WithTimeout.Do(timeout, ScrollPageAndCollect);
    }

    private async Task<IReadOnlyList<IWebElement>> ScrollPageAndCollect(CancellationToken cancellationToken)
    {
        var result = default(IReadOnlyList<IWebElement>);
        
        while (result is null)
        {
            ClickShowMoreIconIfExists();

            var actions = new Actions(_driver);
            actions.ScrollByAmount(0, 100);
            actions.Perform();

            var loadedOffers = GetLoadedOffers();

            if (loadedOffers.Count != _collectedElementsCount)
            {
                var lastOffer = loadedOffers.Last();
                actions.Reset();
                actions.MoveToElement(lastOffer);
                actions.Perform();

                result = loadedOffers
                    .Skip(_collectedElementsCount)
                    .ToArray();
                
                _collectedElementsCount = loadedOffers.Count;
            }

            await Task.Delay(500, cancellationToken);
        }

        return result;
    }

    private void ClickShowMoreIconIfExists()
    {
        if (_driver.TryFindElement(By.ClassName("pokaz-wiecej__icon"), out var showMoreElement))
        {
            showMoreElement.Click();
        }
    }

    private ReadOnlyCollection<IWebElement> GetLoadedOffers() => 
        _driver.FindElements(By.ClassName("bloczek__container"));
}
