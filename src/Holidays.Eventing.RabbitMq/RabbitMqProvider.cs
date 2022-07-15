using RabbitMQ.Client;

namespace Holidays.Eventing.RabbitMq;

public sealed class RabbitMqProvider : IExternalProvider
{
    private readonly RabbitMqSettings _settings;
    private readonly RabbitMqProviderOptions _options;
    private readonly Guid _clientId = Guid.NewGuid();

    private IConnection? _connection;
    private ChannelFactory? _channelFactory;
    private IExternalEventSource? _source;
    private IExternalEventSink? _sink;

    public RabbitMqProvider(RabbitMqSettings settings, RabbitMqProviderOptions options)
    {
        _settings = settings;
        _options = options;
    }

    public IExternalEventSource Source =>
        _source ?? throw new InvalidOperationException("Provider has not been initialized.");

    public IExternalEventSink Sink =>
        _sink ?? throw new InvalidOperationException("Provider has not been initialized.");

    private IConnection Connection =>
        _connection ?? throw new NullReferenceException($"{nameof(Connection)} is null.");

    private IModel Channel =>
        _channelFactory?.GetOrCreateChannel() ?? throw new NullReferenceException($"{nameof(Channel)} is null.");

    public async Task Initialize(EventBus eventBus)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            VirtualHost = _settings.VirtualHost,
            UserName = _settings.UserName,
            Password = _settings.Password,
            ClientProvidedName = _clientId.ToString(),
            DispatchConsumersAsync = true
        };

        _connection = connectionFactory.CreateConnection();
        _channelFactory = new ChannelFactory(_connection);

        SetupQueue(_channelFactory.GetOrCreateChannel());

        var eventConverter = new EventConverter(eventBus.GetRegisteredEventTypes());

        _sink = new RabbitMqSink(_options, _clientId, _channelFactory, eventConverter);
        _source = new RabbitMqSource(_options, _clientId, _channelFactory, eventBus, eventConverter);

        await _source.Setup();
    }

    public ValueTask DisposeAsync()
    {
        Channel.Dispose();
        Connection.Dispose();

        return ValueTask.CompletedTask;
    }

    private void SetupQueue(IModel channel)
    {
        var queueName = $"{Constants.QueuePrefix}{_clientId.ToString()}";

        channel.ExchangeDeclare(
            Constants.Exchange,
            ExchangeType.Fanout,
            durable: true,
            autoDelete: false);

        channel.QueueDeclare(
            queueName,
            durable: false,
            autoDelete: true,
            exclusive: true);

        channel.QueueBind(
            queueName,
            Constants.Exchange,
            routingKey: string.Empty);
    }
}
