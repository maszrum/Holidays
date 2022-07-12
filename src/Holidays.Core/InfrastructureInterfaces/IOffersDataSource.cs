using Holidays.Core.OfferModel;

namespace Holidays.Core.InfrastructureInterfaces;

public interface IOffersDataSource
{
    string WebsiteName { get; }
    
    Task<Result<Offers>> GetOffers(Predicate<Offer> predicate);
}
