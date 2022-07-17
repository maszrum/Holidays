namespace Holidays.Core.Events.OfferModel;

public record OfferData(
    string Hotel,
    string Destination,
    DateOnly DepartureDate,
    int Days,
    string CityOfDeparture,
    int Price,
    string DetailsUrl,
    string WebsiteName);
