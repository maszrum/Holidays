namespace Holidays.Core.OfferModel;

// ReSharper disable EnforceIfStatementBraces

internal class OfferComparerIgnoresPrice : IEqualityComparer<Offer>
{
    public bool Equals(Offer? x, Offer? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;

        return x.Hotel == y.Hotel &&
               x.Destination == y.Destination &&
               x.DepartureDate.Equals(y.DepartureDate) &&
               x.Days == y.Days &&
               x.CityOfDeparture == y.CityOfDeparture &&
               x.WebsiteName == y.WebsiteName;
    }

    public int GetHashCode(Offer obj)
    {
        return HashCode.Combine(
            obj.Hotel,
            obj.Destination,
            obj.DepartureDate,
            obj.Days,
            obj.CityOfDeparture,
            obj.WebsiteName);
    }
}
