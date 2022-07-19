using Holidays.BlazorUI.Services;
using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;

namespace Holidays.BlazorUI.Eventing;

internal class OfferPriceChangedEventHandler : IEventHandler<OfferPriceChanged>
{
    private readonly OffersService _offersService;

    public OfferPriceChangedEventHandler(OffersService offersService)
    {
        _offersService = offersService;
    }

    public async Task Handle(OfferPriceChanged @event, Func<Task> next, CancellationToken cancellationToken)
    {
        await _offersService.Invoke(
            @event.OfferId,
            offer => offer.ChangePrice(@event.CurrentPrice));

        await next();
    }
}
