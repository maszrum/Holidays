using Holidays.Core.Eventing;

namespace Holidays.Eventing.RabbitMq;

internal class RabbitMqSink : IExternalEventSink
{
    private readonly RabbitMqProviderOptions _options;
    private readonly Guid _publisherId;
    private readonly ChannelFactory _channelFactory;
    private readonly EventConverter _eventConverter;

    public RabbitMqSink(
        RabbitMqProviderOptions options,
        Guid publisherId, 
        ChannelFactory channelFactory, 
        EventConverter eventConverter)
    {
        _options = options;
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
        
        _options.EventSentLogAction?.Invoke(@event);

        return Task.CompletedTask;
    }
}
