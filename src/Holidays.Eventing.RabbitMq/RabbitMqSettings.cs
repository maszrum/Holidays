using Holidays.Configuration;

namespace Holidays.Eventing.RabbitMq;

public class RabbitMqSettingsDescriptor : ISettingsDescriptor
{
    public string Section => "RabbitMq";
}

public class RabbitMqSettings : ISettings<RabbitMqSettingsDescriptor>
{
    public bool UseRabbitMq { get; init; }

    public string HostName { get; init; } = null!;

    public int Port { get; init; }

    public string VirtualHost { get; init; } = string.Empty;

    public string UserName { get; init; } = null!;

    public string Password { get; init; } = null!;

    public bool IsValid()
    {
        if (!UseRabbitMq)
        {
            return true;
        }

        return !string.IsNullOrWhiteSpace(HostName) &&
               Port > 0 &&
               !string.IsNullOrWhiteSpace(UserName) &&
               !string.IsNullOrWhiteSpace(Password);
    }
}
