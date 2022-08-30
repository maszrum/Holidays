using System.Collections;
using System.Collections.Immutable;

namespace Holidays.Core.OfferModel;

public class Offers : IEnumerable<Offer>
{
    public Offers(IEnumerable<Offer> elements)
    {
        Elements = ImmutableHashSet.CreateRange(new OfferComparerIgnoresPriceAndCityOfDeparture(), elements);
    }

    public ImmutableHashSet<Offer> Elements { get; }

    public IEnumerator<Offer> GetEnumerator() => Elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Elements.GetEnumerator();
}
