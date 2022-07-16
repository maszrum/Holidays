namespace Holidays.DataTransferObjects;

public class OfferDto
{
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
