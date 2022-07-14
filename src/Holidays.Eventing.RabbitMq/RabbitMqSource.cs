using System.Text;
using Holidays.Core.Eventing;
using RabbitMQ.Client.Events;

namespace Holidays.Eventing.RabbitMq;

internal class RabbitMqSource : IExternalEventSource
{
    private readonly RabbitMqProviderOptions _options;
    private readonly Guid _consumerId;
    private readonly ChannelFactory _channelFactory;
    private readonly EventBus _eventBus;
    private readonly EventConverter _eventConverter;

    public RabbitMqSource(
        RabbitMqProviderOptions options,
        Guid consumerId,
        ChannelFactory channelFactory,
        EventBus eventBus, 
        EventConverter eventConverter)
    {
        _options = options;
        _consumerId = consumerId;
        _channelFactory = channelFactory;
        _eventBus = eventBus;
        _eventConverter = eventConverter;
        _options = options;
    }

    public Task Setup()
    {
        var channel = _channelFactory.GetOrCreateChannel();
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += ProcessMessageAndPublishToLocalBus;

        var queueName = $"{Constants.QueuePrefix}{_consumerId.ToString()}";

        channel.BasicConsume(
            queue: queueName,
            autoAck: true,
            consumerTag: _consumerId.ToString(),
            noLocal: true,
            exclusive: true,
            arguments: null,
            consumer: consumer);

        return Task.CompletedTask;
    }

    private Task ProcessMessageAndPublishToLocalBus(object sender, BasicDeliverEventArgs args)
    {
        var publisherId = GetPublisherIdFromHeaders(args.BasicProperties.Headers);

        if (publisherId == _consumerId)
        {
            return Task.CompletedTask;
        }

        var eventTypeName = GetEventTypeNameFromHeaders(args.BasicProperties.Headers);

        if (!_eventConverter.IsTypeRegistered(eventTypeName))
        {
            return _options.UnknownEventTypeAction is null 
                ? Task.CompletedTask 
                : _options.UnknownEventTypeAction(eventTypeName);
        }

        IEvent @event;
        try
        {
            @event = _eventConverter.ConvertToEvent(eventTypeName, args.Body.Span);
        }
        catch (Exception exception)
        {
            return _options.EventDeserializationError is null
                ? Task.CompletedTask
                : _options.EventDeserializationError(exception, eventTypeName);
        }

        return _eventBus.PublishAsExternalProvider(@event, CancellationToken.None);
    }

    private static Guid GetPublisherIdFromHeaders(IDictionary<string, object> headers)
    {
        var publisherIdBytes = (byte[])headers[Constants.EventPublisherHeader];
        var publisherIdHeader = Encoding.UTF8.GetString(publisherIdBytes);
        var publisherId = Guid.Parse(publisherIdHeader);
        
        return publisherId;
    }

    private static string GetEventTypeNameFromHeaders(IDictionary<string, object> headers)
    {
        var eventTypeBytes = (byte[])headers[Constants.EventTypeHeader];
        var eventTypeName = Encoding.UTF8.GetString(eventTypeBytes);

        return eventTypeName;
    }
}
