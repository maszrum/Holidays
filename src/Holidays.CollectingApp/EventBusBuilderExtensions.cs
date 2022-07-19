using Holidays.Core.Events.OfferModel;
using Holidays.Eventing;
using Holidays.Eventing.RabbitMq;
using Holidays.InMemoryStore;
using Holidays.InMemoryStore.EventHandlers;
using Holidays.Postgres;
using Holidays.Postgres.EventHandlers;

namespace Holidays.CollectingApp;

internal static class EventBusBuilderExtensions
{
    public static EventBusBuilder RegisterEventHandlers(
        this EventBusBuilder builder,
        InMemoryDatabase inMemoryDatabase,
        PostgresConnectionFactory postgresConnectionFactory)
    {
        builder
            .ForEventType<OfferAdded>()
            .RegisterHandler(() => new OfferAddedInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandlerForLocalEventsOnly(() => new OfferAddedPostgresEventHandler(postgresConnectionFactory));

        builder
            .ForEventType<OfferRemoved>()
            .RegisterHandler(() => new OfferRemovedInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandlerForLocalEventsOnly(() => new OfferRemovedPostgresEventHandler(postgresConnectionFactory));

        builder
            .ForEventType<OfferPriceChanged>()
            .RegisterHandler(() => new OfferPriceChangedInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandlerForLocalEventsOnly(() => new OfferPriceChangedPostgresEventHandler(postgresConnectionFactory));

        builder
            .ForEventType<OfferStartedTracking>()
            .RegisterHandler(() => new OfferStartedTrackingInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandlerForLocalEventsOnly(() => new OfferStartedTrackingPostgresEventHandler(postgresConnectionFactory));

        return builder;
    }

    public static EventBusBuilder ConfigureRabbitMqIfTurnedOn(
        this EventBusBuilder builder,
        ApplicationBootstrapper app)
    {
        var rabbitMqSettings = app.Configuration.Get<RabbitMqSettings>();

        if (rabbitMqSettings.UseRabbitMq)
        {
            builder.UseRabbitMq(rabbitMqSettings, options =>
            {
                options.OnUnknownEventType(eventTypeName =>
                {
                    app.GetLogger<RabbitMqProvider>().Error(
                        "Received event with unknown event type: {EventType}",
                        eventTypeName);
                });
                options.OnDeserializationError((exception, eventTypeName) =>
                {
                    app.GetLogger<RabbitMqProvider>().Error(
                        exception,
                        "An error occured on deserializing event data {EventType}",
                        eventTypeName);
                });
                options.OnEventReceived(@event =>
                {
                    app.GetLogger<RabbitMqProvider>().Debug(
                        "Received event from external broker: {EventType}",
                        @event.GetType().Name);
                });
                options.OnEventSent(@event =>
                {
                    app.GetLogger<RabbitMqProvider>().Debug(
                        "Sent event to external broker: {EventType}",
                        @event.GetType().Name);
                });
            });
        }

        return builder;
    }
}
