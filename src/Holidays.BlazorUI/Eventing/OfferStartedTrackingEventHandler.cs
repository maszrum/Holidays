using Holidays.BlazorUI.Services;
using Holidays.BlazorUI.ViewModels;
using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;

namespace Holidays.BlazorUI.Eventing;

internal class OfferStartedTrackingEventHandler : IEventHandler<OfferStartedTracking>
{
    private readonly OffersService _offersService;

    public OfferStartedTrackingEventHandler(OffersService offersService)
    {
        _offersService = offersService;
    }

    public async Task Handle(OfferStartedTracking @event, Func<Task> next, CancellationToken cancellationToken)
    {
        var viewModel = OfferViewModel.FromEvent(@event);

        await _offersService.Add(viewModel);

        await next();
    }
}
