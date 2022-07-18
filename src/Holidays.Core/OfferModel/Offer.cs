using System.Diagnostics;

namespace Holidays.Core.OfferModel;

[DebuggerDisplay("{DestinationCountry}, {DepartureDate}, {Price}")]
public class Offer
{
    private static readonly OfferIdGenerator IdGenerator = new();

    private readonly Lazy<Guid> _guid;

    public Offer(
        string hotel,
        string destinationCountry,
        string detailedDestination,
        DateOnly departureDate,
        int days,
        string cityOfDeparture,
        int price,
        string detailsUrl,
        string websiteName)
    {
        Hotel = hotel;
        DestinationCountry = destinationCountry;
        DetailedDestination = detailedDestination;
        DepartureDate = departureDate;
        Days = days;
        CityOfDeparture = cityOfDeparture;
        Price = price;
        DetailsUrl = detailsUrl;
        WebsiteName = websiteName;

        _guid = new Lazy<Guid>(() => IdGenerator.Generate(this));
    }

    public Guid Id => _guid.Value;

    public string Hotel { get; init; }

    public string DestinationCountry { get; init; }

    public string DetailedDestination { get; init; }

    public DateOnly DepartureDate { get; init; }

    public int Days { get; init; }

    public string CityOfDeparture { get; init; }

    public int Price { get; init; }

    public string DetailsUrl { get; init; }

    public string WebsiteName { get; init; }
}
