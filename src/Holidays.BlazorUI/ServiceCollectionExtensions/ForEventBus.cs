using Holidays.Configuration;
using Holidays.Core.Events.OfferModel;
using Holidays.Eventing;
using Holidays.Eventing.RabbitMq;
using Holidays.InMemoryStore;
using Holidays.InMemoryStore.EventHandlers;
using Holidays.BlazorUI.Eventing;
using Holidays.BlazorUI.Services;

namespace Holidays.BlazorUI.ServiceCollectionExtensions;

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
        var offersService = serviceProvider.GetRequiredService<OffersService>();

        builder
            .ForEventType<OfferAdded>()
            .RegisterHandler(() => new OfferAddedInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandler(() => new OfferAddedEventHandler(offersService));

        builder
            .ForEventType<OfferRemoved>()
            .RegisterHandler(() => new OfferRemovedInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandler(() => new OfferRemovedEventHandler(offersService));

        builder
            .ForEventType<OfferPriceChanged>()
            .RegisterHandler(() => new OfferPriceChangedInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandler(() => new OfferPriceChangedEventHandler(offersService));

        builder
            .ForEventType<OfferStartedTracking>()
            .RegisterHandler(() => new OfferStartedTrackingInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandler(() => new OfferStartedTrackingEventHandler(offersService));

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
