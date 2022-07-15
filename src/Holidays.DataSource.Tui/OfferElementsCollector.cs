using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Holidays.Selenium;
using OpenQA.Selenium;

namespace Holidays.DataSource.Tui;

internal class OfferElementsCollector
{
    private readonly IWebDriver _driver;
    private DateOnly? _lastCollectedDay;

    public OfferElementsCollector(IWebDriver driver)
    {
        _driver = driver;
    }

    public Task<IReadOnlyList<IWebElement>> Collect(TimeSpan timeout) =>
        WithTimeout.Do(timeout, SwitchPageIfNeedAndCollectOffers);

    private async Task<IReadOnlyList<IWebElement>> SwitchPageIfNeedAndCollectOffers(CancellationToken cancellationToken)
    {
        while (!TryGetCurrentDayElement(out _))
        {
            await Task.Delay(1_000, cancellationToken);
        }

        if (_lastCollectedDay.HasValue)
        {
            var nextDay = _lastCollectedDay.Value.AddDays(1);
            var nextDayElement = FindDayElement(nextDay);
            nextDayElement.Click();
        }

        IWebElement currentDayElement;
        while (!TryGetCurrentDayElement(out currentDayElement!))
        {
            await Task.Delay(1_000, cancellationToken);
        }

        _lastCollectedDay = GetDateOnlyFromDayElement(currentDayElement);

        var elements = await CollectFromCurrentPage(CancellationToken.None);

        return elements;
    }

    private async Task<IReadOnlyList<IWebElement>> CollectFromCurrentPage(CancellationToken cancellationToken)
    {
        while (!IsNoMoreOffersTextDisplayed())
        {
            ClickShowMoreIconIfExists();
            await Task.Delay(1_000, cancellationToken);
        }

        var loadedOfferElements = GetLoadedOffers();

        return loadedOfferElements;
    }

    private void ClickShowMoreIconIfExists()
    {
        if (_driver.TryFindElement(By.ClassName("results-container__button"), out var showMoreElement))
        {
            showMoreElement.Click();
        }
    }

    private bool TryGetCurrentDayElement([NotNullWhen(true)] out IWebElement? element) =>
        _driver.TryFindElement(By.CssSelector("div.upcoming-offers-tile--active span"), out element);

    private IWebElement FindDayElement(DateOnly date)
    {
        var daysElements = _driver.FindElements(By.CssSelector("div.upcoming-offers-tile span"));

        foreach (var dayElement in daysElements)
        {
            var dayDate = GetDateOnlyFromDayElement(dayElement);
            if (dayDate == date)
            {
                return dayElement;
            }
        }

        throw new InvalidOperationException(
            $"Cannot find day element for specified date: {date}");
    }

    private bool IsNoMoreOffersTextDisplayed() =>
        _driver.TryFindElement(By.ClassName("results-container__no-more-text"), out _);

    private ReadOnlyCollection<IWebElement> GetLoadedOffers() =>
        _driver.FindElements(By.ClassName("offer-tile--listingOffer"));

    private static DateOnly GetDateOnlyFromDayElement(IWebElement element)
    {
        var dateText = element.Text;

        // i wish i could regex...

        var indexOfSeparator = dateText.IndexOf(" - ", StringComparison.Ordinal);

        if (indexOfSeparator == -1)
        {
            throw new FormatException(
                $"Cannot read day from element: '{dateText}'.");
        }

        var cutDateText = dateText.Substring(indexOfSeparator + 3);
        var indexOfDot = cutDateText.IndexOf('.');

        if (indexOfDot == -1)
        {
            throw new FormatException(
                $"Cannot read day from element: '{dateText}'.");
        }

        var day = cutDateText.Substring(0, indexOfDot);
        var month = cutDateText.Substring(indexOfDot + 1);

        return new DateOnly(
            DateTime.UtcNow.Year,
            int.Parse(month),
            int.Parse(day));
    }
}
