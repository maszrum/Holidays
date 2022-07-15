namespace Holidays.InMemoryStore.DbRecords;

internal record OfferDbRecord
{
    // ReSharper disable once UnusedMember.Global
    public OfferDbRecord()
    {
    }

    public OfferDbRecord(Guid id,
        string hotel,
        string destination,
        DateOnly departureDate,
        int days,
        string cityOfDeparture,
        int price,
        string detailsUrl,
        string websiteName)
    {
        Id = id;
        Hotel = hotel;
        Destination = destination;
        DepartureDate = departureDate;
        Days = days;
        CityOfDeparture = cityOfDeparture;
        Price = price;
        DetailsUrl = detailsUrl;
        WebsiteName = websiteName;
    }

    public Guid Id { get; init; }

    public string Hotel { get; init; } = null!;

    public string Destination { get; init; } = null!;

    public DateOnly DepartureDate { get; init; }

    public int Days { get; init; }

    public string CityOfDeparture { get; init; } = null!;

    public int Price { get; init; }

    public string DetailsUrl { get; init; } = null!;

    public string WebsiteName { get; init; } = null!;
}
