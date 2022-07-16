using Holidays.Configuration;
using Holidays.Core.OfferModel;
using Holidays.Eventing;
using Holidays.Eventing.RabbitMq;
using Holidays.InMemoryStore;
using Holidays.InMemoryStore.EventHandlers;

namespace Holidays.WebAPI.ServiceCollectionExtensions;

internal static class ForEventBus
{
    public static IServiceCollection AddEventBus(this IServiceCollection services)
    {
        return services.AddSingleton(serviceProvider =>
        {
            var eventBus = new EventBusBuilder()
                .UseRabbitMq(serviceProvider)
                .RegisterEventHandlers(serviceProvider)
                .Build();

            return eventBus;
        });
    }

    private static EventBusBuilder RegisterEventHandlers(this EventBusBuilder builder, IServiceProvider serviceProvider)
    {
        var inMemoryDatabase = serviceProvider.GetRequiredService<InMemoryDatabase>();

        builder
            .ForEventType<OfferAdded>()
            .RegisterHandlerForAllEvents(() => new OfferAddedInMemoryStoreEventHandler(inMemoryDatabase));

        builder
            .ForEventType<OfferRemoved>()
            .RegisterHandlerForAllEvents(() => new OfferRemovedInMemoryStoreEventHandler(inMemoryDatabase));

        builder
            .ForEventType<OfferPriceChanged>()
            .RegisterHandlerForAllEvents(() => new OfferPriceChangedInMemoryStoreEventHandler(inMemoryDatabase));

        builder
            .ForEventType<OfferStartedTracking>()
            .RegisterHandlerForAllEvents(() => new OfferStartedTrackingInMemoryStoreEventHandler(inMemoryDatabase));

        return builder;
    }

    private static EventBusBuilder UseRabbitMq(this EventBusBuilder builder, IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<ApplicationConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<RabbitMqProvider>>();
        var rabbitMqSettings = configuration.Get<RabbitMqSettings>();

        return builder.UseRabbitMq(rabbitMqSettings, options =>
        {
            options.OnUnknownEventType(eventTypeName =>
            {
                logger.LogError(
                    "Received event with unknown event type: {EventType}",
                    eventTypeName);
            });
            options.OnDeserializationError((exception, eventTypeName) =>
            {
                logger.LogError(
                    exception,
                    "An error occured on deserializing event data {EventType}",
                    eventTypeName);
            });
            options.OnEventReceived(@event =>
            {
                logger.LogDebug(
                    "Received event from external broker: {EventType}",
                    @event.GetType().Name);
            });
            options.OnEventSent(@event =>
            {
                logger.LogDebug(
                    "Sent event to external broker: {EventType}",
                    @event.GetType().Name);
            });
        });
    }
}
