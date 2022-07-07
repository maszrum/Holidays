using Holidays.Core.OfferModel;

namespace Holidays.Core.InfrastructureInterfaces;

public interface IOffersDataSource
{
    Task<Result<Offers>> GetOffers(Predicate<Offer> predicate);
}
