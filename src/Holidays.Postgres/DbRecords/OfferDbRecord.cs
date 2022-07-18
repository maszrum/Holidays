namespace Holidays.Postgres.DbRecords;

internal class OfferDbRecord
{
    // ReSharper disable once UnusedMember.Local
    private OfferDbRecord()
    {
    }

    public OfferDbRecord(
        Guid id,
        string hotel,
        string destinationCountry,
        string detailedDestination,
        int departureDate,
        int days,
        string cityOfDeparture,
        int price,
        string detailsUrl,
        string websiteName,
        bool isRemoved)
    {
        Id = id;
        Hotel = hotel;
        DestinationCountry = destinationCountry;
        DetailedDestination = detailedDestination;
        DepartureDate = departureDate;
        Days = days;
        CityOfDeparture = cityOfDeparture;
        Price = price;
        DetailsUrl = detailsUrl;
        WebsiteName = websiteName;
        IsRemoved = isRemoved;
    }

    public Guid Id { get; init; }

    public string Hotel { get; init; } = null!;

    public string DestinationCountry { get; init; } = null!;

    public string DetailedDestination { get; init; } = null!;

    public int DepartureDate { get; init; }

    public int Days { get; init; }

    public string CityOfDeparture { get; init; } = null!;

    public int Price { get; init; }

    public string DetailsUrl { get; init; } = null!;

    public string WebsiteName { get; init; } = null!;

    public bool IsRemoved { get; init; }
}
