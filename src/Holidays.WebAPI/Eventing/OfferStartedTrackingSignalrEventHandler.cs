using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;
using Microsoft.AspNetCore.SignalR;

namespace Holidays.WebAPI.Eventing;

internal class OfferStartedTrackingSignalrEventHandler : IEventHandler<OfferStartedTracking>
{
    private readonly IHubContext<EventsHub> _hub;

    public OfferStartedTrackingSignalrEventHandler(IHubContext<EventsHub> hub)
    {
        _hub = hub;
    }

    public async Task Handle(OfferStartedTracking @event, Func<Task> next, CancellationToken cancellationToken)
    {
        await _hub.Clients.All.SendAsync(nameof(OfferStartedTracking), cancellationToken);

        await next();
    }
}
