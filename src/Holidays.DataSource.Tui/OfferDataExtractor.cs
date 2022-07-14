using Holidays.Core.OfferModel;
using OpenQA.Selenium;

namespace Holidays.DataSource.Tui;

internal class OfferDataExtractor
{
    public Offer Extract(IWebElement element)
    {
        var hotelElement = element.FindElement(By.ClassName("offer-tile-body__hotel-name"));
        
        var destination = GetDestination(element);

        var (departureDate, days) = GetDepartureDateAndDays(element);

        var cityOfDepartureElement = element.FindElement(By.CssSelector("button.dropdown-field--same-day-offers span"));

        var priceElement = element.FindElement(By.ClassName("price-value__amount"));
        
        var detailsLinkElement = element.FindElement(By.ClassName("offer-tile-aside__button--cta"));

        var offer = new Offer(
            hotel: hotelElement.Text,
            destination: destination,
            departureDate: departureDate,
            days: days,
            cityOfDeparture: cityOfDepartureElement.Text,
            price: int.Parse(priceElement.Text.Replace(" ", string.Empty)),
            detailsUrl: detailsLinkElement.GetAttribute("href"),
            Constants.WebsiteName);

        return offer;
    }

    private static string GetDestination(IWebElement offerElement)
    {
        var elements = offerElement.FindElements(By.CssSelector("li.breadcrumbs__item a"));

        var texts = elements
            .Select(e => e.Text)
            .ToArray();

        return string.Join(" / ", texts);
    }

    private static (DateOnly, int) GetDepartureDateAndDays(IWebElement offerElement)
    {
        var spanElements = offerElement.FindElements(By.CssSelector("ul.offer-tile-body__info-list span"));

        var element = spanElements.Single(e =>
        {
            var text = e.Text;
            return text.Length > 0 && char.IsDigit(text[0]);
        });

        var text = element.Text;

        var indexOfSpace = text.IndexOf(' ');
        var departureDateText = text.Substring(0, indexOfSpace);

        var indexOfBracket = text.IndexOf('(');
        var daysText = text.Substring(indexOfBracket + 1);
        indexOfSpace = daysText.IndexOf(' ');
        daysText = daysText.Substring(0, indexOfSpace);

        return (
            DateOnly.ParseExact(departureDateText, "dd.MM.yyyy"), 
            int.Parse(daysText));
    }
}
