using Holidays.Core.OfferModel;

namespace Holidays.Core.InfrastructureInterfaces;

public interface IOffersRepository
{
    Task<Offers> GetAll();
    
    Task<Offers> GetAllByWebsiteName(string websiteName);

    Task<Offers> GetAllRemovedByWebsiteName(string websiteName);
    
    Task<Maybe<Offer>> Get(Guid offerId);

    Task<Maybe<DateOnly>> GetLastDepartureDate(string websiteName);
}
