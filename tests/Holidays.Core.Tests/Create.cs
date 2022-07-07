using Holidays.Core.OfferModel;

namespace Holidays.Core.Tests;

internal static class Create
{
    public static Offers Offers(params Offer[] offers) => new(offers);
}
