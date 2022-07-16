using Holidays.Configuration;

namespace Holidays.Eventing.RabbitMq.Tests;

internal static class Create
{
    public static async Task<IEventBus> EventBus(
        Action<EventBusBuilder> builderAction,
        Action<string>? onUnknownEventType = default,
        Action<Exception, string>? onDeserializationError = default)
    {
        var rabbitMqSettings = ReadSettings();

        var eventBusBuilder = new EventBusBuilder()
            .UseRabbitMq(rabbitMqSettings, options =>
            {
                if (onDeserializationError is not null)
                {
                    options.OnDeserializationError(onDeserializationError);
                }

                if (onUnknownEventType is not null)
                {
                    options.OnUnknownEventType(onUnknownEventType);
                }
            });

        builderAction(eventBusBuilder);

        var eventBus = await eventBusBuilder
            .Build()
            .Initialize();

        return eventBus;
    }

    private static RabbitMqSettings ReadSettings()
    {
        var configuration = new ApplicationConfiguration(
            "testsettings.json",
            overrideWithEnvironmentVariables: false);

        return configuration.Get<RabbitMqSettings>();
    }
}
