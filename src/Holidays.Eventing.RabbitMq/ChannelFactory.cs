using System.Runtime.CompilerServices;
using RabbitMQ.Client;

namespace Holidays.Eventing.RabbitMq;

internal sealed class ChannelFactory : IDisposable
{
    private readonly IConnection _connection;

    private IModel? _channel;

    public ChannelFactory(IConnection connection)
    {
        _connection = connection;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IModel GetOrCreateChannel()
    {
        if (_channel is not null && _channel.IsOpen)
        {
            return _channel;
        }

        _channel = _connection.CreateModel();
        return _channel;
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
