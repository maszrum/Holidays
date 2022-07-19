using Holidays.Core.Events.OfferModel;
using Holidays.Eventing.Core;
using Microsoft.AspNetCore.SignalR;

namespace Holidays.WebAPI.Eventing;

internal class OfferPriceChangedSignalrEventHandler : IEventHandler<OfferPriceChanged>
{
    private readonly IHubContext<EventsHub> _hub;

    public OfferPriceChangedSignalrEventHandler(IHubContext<EventsHub> hub)
    {
        _hub = hub;
    }

    public async Task Handle(OfferPriceChanged @event, Func<Task> next, CancellationToken cancellationToken)
    {
        await _hub.Clients.All.SendAsync(nameof(OfferPriceChanged), @event, cancellationToken);

        await next();
    }
}
