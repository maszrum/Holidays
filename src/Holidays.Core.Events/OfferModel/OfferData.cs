namespace Holidays.Core.Events.OfferModel;

public record OfferData(
    string Hotel,
    string DestinationCountry,
    string DetailedDestination,
    DateOnly DepartureDate,
    int Days,
    string CityOfDeparture,
    int Price,
    string DetailsUrl,
    string WebsiteName);
