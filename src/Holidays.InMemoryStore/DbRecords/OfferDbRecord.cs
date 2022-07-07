namespace Holidays.InMemoryStore.DbRecords;

internal record OfferDbRecord(
    Guid Id,
    string Hotel,
    string Destination,
    DateOnly DepartureDate,
    int Days,
    string CityOfDeparture,
    int Price,
    string DetailsUrl,
    bool IsRemoved);
