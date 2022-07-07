using Holidays.Core.OfferModel;

namespace Holidays.Core.InfrastructureInterfaces;

public interface IOffersRepository
{
    Task<Offers> GetAll();

    Task<Offers> GetAllRemoved();

    Task<Maybe<Offer>> TryGetRemoved(Guid offerId);
}
