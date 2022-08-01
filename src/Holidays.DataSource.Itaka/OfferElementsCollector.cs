using System.Collections.ObjectModel;
using Holidays.Selenium;
using OpenQA.Selenium;

namespace Holidays.DataSource.Itaka;

internal class OfferElementsCollector
{
    private readonly IWebDriver _driver;
    private int _collectedElementsCount;

    public OfferElementsCollector(IWebDriver webDriver)
    {
        _driver = webDriver;
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

        while (result is null && !cancellationToken.IsCancellationRequested)
        {
            ClickShowMoreIconIfExists();
            ClickCloseAdButtonIfExists();

            var loadedOffers = GetLoadedOffers();

            if (loadedOffers.Count != _collectedElementsCount)
            {
                result = loadedOffers
                    .Skip(_collectedElementsCount)
                    .ToArray();

                _collectedElementsCount = loadedOffers.Count;
            }

            await Task.Delay(500, cancellationToken);
        }

        return result ?? throw new OperationCanceledException();
    }

    private void ClickShowMoreIconIfExists()
    {
        if (_driver.TryFindElement(By.ClassName("offer-list_more-offers"), out var closeElement))
        {
            closeElement.Click();
        }
    }

    private void ClickCloseAdButtonIfExists()
    {
        if (!_driver.TryFindElement(By.Id("pushAd_disagree_button"), out var closeElement))
        {
            return;
        }

        try
        {
            closeElement.Click();
        }
        catch (ElementNotInteractableException)
        {
            // ignore
        }
    }

    private ReadOnlyCollection<IWebElement> GetLoadedOffers() =>
        _driver.FindElements(By.ClassName("offer_column-second"));
}
