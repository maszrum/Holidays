using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;
using Microsoft.AspNetCore.SignalR;

namespace Holidays.WebAPI.Eventing;

internal class OfferRemovedSignalrEventHandler : IEventHandler<OfferRemoved>
{
    private readonly IHubContext<EventsHub> _hub;

    public OfferRemovedSignalrEventHandler(IHubContext<EventsHub> hub)
    {
        _hub = hub;
    }

    public async Task Handle(OfferRemoved @event, Func<Task> next, CancellationToken cancellationToken)
    {
        await _hub.Clients.All.SendAsync(nameof(OfferRemoved), @event, cancellationToken);

        await next();
    }
}
