using Holidays.Core.OfferModel;

namespace Holidays.Core.InfrastructureInterfaces;

public interface IPriceHistoryRepository
{
    Task<Maybe<PriceHistory>> Get(Guid offerId);
}
