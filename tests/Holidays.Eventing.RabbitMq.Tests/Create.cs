﻿using Holidays.Configuration;

namespace Holidays.Eventing.RabbitMq.Tests;

internal static class Create
{
    public static async Task<IEventBus> EventBus(
        Action<EventBusBuilder> builderAction, 
        Func<string, Task>? onUnknownEventType = default, 
        Func<Exception, string, Task>? onDeserializationError = default)
    {
        var rabbitMqSettings = ReadSettings();

        var eventBusBuilder = new EventBusBuilder()
            .UseRabbitMq(rabbitMqSettings, options =>
            {
                options.AddEventTypesAssembly<TestEvent>();

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

        var eventBus = await eventBusBuilder.Build();

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
