using Holidays.Core.OfferModel;

namespace Holidays.Core.InfrastructureInterfaces;

public interface IOffersRepository
{
    Task<Offers> GetAllByWebsiteName(string websiteName);

    Task<Maybe<DateOnly>> GetLastDepartureDate(string websiteName);
}
