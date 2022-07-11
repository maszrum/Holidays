using Holidays.Core.OfferModel;

namespace Holidays.Core.InfrastructureInterfaces;

public interface IPriceHistoryRepository
{
    Task<PriceHistory> Get(Guid offerId);
}
