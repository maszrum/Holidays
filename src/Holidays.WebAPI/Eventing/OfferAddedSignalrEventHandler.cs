using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;
using Microsoft.AspNetCore.SignalR;

namespace Holidays.WebAPI.Eventing;

internal class OfferAddedSignalrEventHandler : IEventHandler<OfferAdded>
{
    private readonly IHubContext<EventsHub> _hub;

    public OfferAddedSignalrEventHandler(IHubContext<EventsHub> hub)
    {
        _hub = hub;
    }

    public async Task Handle(OfferAdded @event, Func<Task> next, CancellationToken cancellationToken)
    {
        await _hub.Clients.All.SendAsync(nameof(OfferAdded), @event, cancellationToken);

        await next();
    }
}
