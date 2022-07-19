using Holidays.BlazorUI.Services;
using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;

namespace Holidays.BlazorUI.Eventing;

internal class OfferRemovedEventHandler : IEventHandler<OfferRemoved>
{
    private readonly OffersService _offersService;

    public OfferRemovedEventHandler(OffersService offersService)
    {
        _offersService = offersService;
    }

    public async Task Handle(OfferRemoved @event, Func<Task> next, CancellationToken cancellationToken)
    {
        await _offersService.Invoke(
            @event.OfferId,
            offer => offer.MarkAsRemoved());

        await next();
    }
}
