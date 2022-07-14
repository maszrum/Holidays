using Holidays.Core.Eventing;
using RabbitMQ.Client;

namespace Holidays.Eventing.RabbitMq;

internal class RabbitMqSink : IExternalEventSink
{
    private readonly Guid _publisherId;
    private readonly ChannelFactory _channelFactory;
    private readonly EventConverter _eventConverter;

    public RabbitMqSink(
        Guid publisherId, 
        ChannelFactory channelFactory, 
        EventConverter eventConverter)
    {
        _publisherId = publisherId;
        _channelFactory = channelFactory;
        _eventConverter = eventConverter;
    }

    public Task Publish(IEvent @event, CancellationToken cancellationToken = default)
    {
        var channel = _channelFactory.GetOrCreateChannel();

        var properties = channel.CreateBasicProperties();
        properties.Headers = new Dictionary<string, object>()
        {
            [Constants.EventTypeHeader] = @event.GetType().Name,
            [Constants.EventPublisherHeader] = _publisherId.ToString()
        };

        var eventBytes = _eventConverter.ConvertToBytes(@event);
        
        channel.BasicPublish(
            Constants.Exchange, 
            routingKey: string.Empty, 
            mandatory: true, 
            basicProperties: properties, 
            body: eventBytes);

        return Task.CompletedTask;
    }
}
