using OpenQA.Selenium;
using Holidays.Core.OfferModel;

namespace Holidays.DataSource.Rainbow;

internal class OfferDataExtractor
{
    public Offer Extract(IWebElement element)
    {
        var hotelElement = element.FindElement(By.CssSelector(".bloczek__naglowek .bloczek__tytul"));
        var destinationElement = element.FindElement(By.CssSelector(".bloczek__lokalizacja .bloczek__lokalizacja--text"));
        
        var parameterElements = element.FindElements(By.CssSelector(".bloczek__content .bloczek__parametr-text"));
        var departureDateAndDaysElement = parameterElements[0];
        var cityOfDepartureElement = parameterElements[1];

        var priceElement = element.FindElement(By.CssSelector(".content__right .bloczek__cena"));
        
        var detailsLinkElement = element.FindElement(By.CssSelector("a.content__right"));

        var (departureDate, days) = ExtractDepartureDateAndDays(departureDateAndDaysElement.Text);

        return new Offer(
            hotel: hotelElement.Text,
            destination: destinationElement.Text,
            departureDate: departureDate,
            days: days,
            cityOfDeparture: cityOfDepartureElement.Text,
            price: int.Parse(priceElement.Text.Replace(" ", string.Empty)),
            detailsUrl: detailsLinkElement.GetAttribute("href"),
            Constants.WebsiteName);
    }

    private static (DateOnly, int) ExtractDepartureDateAndDays(string input)
    {
        var parts = input.Split(' ');

        var date = DateOnly.ParseExact(parts[0], "dd.MM.yyyy");
        var days = int.Parse(parts[1].Substring(1));

        return (date, days);
    }
}
