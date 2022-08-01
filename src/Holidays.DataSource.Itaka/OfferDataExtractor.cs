using Holidays.Core.OfferModel;
using OpenQA.Selenium;

namespace Holidays.DataSource.Itaka;

internal class OfferDataExtractor
{
    public Offer Extract(IWebElement element)
    {
        var hotel = ExtractHotel(element);
        var (destinationCountry, detailedDestination) = ExtractDestination(element);
        var (departureDate, days) = ExtractDepartureDateAndDays(element);
        var price = ExtractPrice(element);
        var detailsLinkElement = element.FindElement(By.ClassName("offer_link"));

        return new Offer(
            hotel: hotel,
            destinationCountry: destinationCountry,
            detailedDestination: detailedDestination,
            departureDate: departureDate,
            days: days,
            cityOfDeparture: string.Empty,
            price: price,
            detailsUrl: detailsLinkElement.GetAttribute("href"),
            websiteName: Constants.WebsiteName);
    }

    private static string ExtractHotel(IWebElement offerElement)
    {
        var linkElements = offerElement.FindElements(By.CssSelector(".header_title a"));
        var hotelElement = linkElements[0];
        var hotel = hotelElement.Text;

        return hotel.StartsWith("Hotel ", StringComparison.Ordinal)
            ? hotel.Substring(6)
            : hotel;
    }

    private static (string, string) ExtractDestination(IWebElement offerElement)
    {
        var destinationElement = offerElement.FindElement(By.ClassName("header_geo-labels"));
        var destinationText = destinationElement.Text;

        var destinationParts = destinationText.Split(
            '/',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var destinationCountry = destinationParts[0];

        var detailedDestination = destinationParts.Length == 1
            ? string.Empty
            : destinationParts[1];

        return (destinationCountry, detailedDestination);
    }

    private static (DateOnly, int) ExtractDepartureDateAndDays(IWebElement offerElement)
    {
        var element = offerElement.FindElement(By.CssSelector(".offer_offer-info-additional .offer_date"));
        var text = element.Text;

        var textParts = text.Split('.', '-', ' ');

        var departureDate = new DateOnly(
            year: 2000 + int.Parse(textParts[4]),
            month: int.Parse(textParts[1]),
            day: int.Parse(textParts[0]));

        var daysText = textParts[5].Substring(1);

        return (departureDate, int.Parse(daysText));
    }

    private static int ExtractPrice(IWebElement offerElement)
    {
        var priceElements = offerElement.FindElements(By.ClassName("current-price_value"));

        var priceText = default(string);

        foreach (var priceElement in priceElements)
        {
            priceText = priceElement.Text;

            if (!string.IsNullOrWhiteSpace(priceText))
            {
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(priceText))
        {
            throw new FormatException("Cannot read offer price.");
        }

        var priceCharacters = priceText
            .Where(char.IsDigit)
            .ToArray();

        var price = int.Parse(new string(priceCharacters));

        return price;
    }
}
