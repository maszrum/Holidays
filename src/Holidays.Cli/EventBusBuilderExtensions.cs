using Holidays.Core.OfferModel;
using Holidays.Eventing;
using Holidays.Eventing.RabbitMq;
using Holidays.InMemoryStore;
using Holidays.InMemoryStore.EventHandlers;
using Holidays.Postgres;
using Holidays.Postgres.EventHandlers;

namespace Holidays.Cli;

internal static class EventBusBuilderExtensions
{
    public static EventBusBuilder RegisterEventHandlers(
        this EventBusBuilder builder,
        InMemoryDatabase inMemoryDatabase,
        PostgresConnectionFactory postgresConnectionFactory)
    {
        builder
            .ForEventType<OfferAdded>()
            .RegisterHandlerForAllEvents(() => new OfferAddedInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandlerForLocalEvents(() => new OfferAddedPostgresEventHandler(postgresConnectionFactory));

        builder
            .ForEventType<OfferRemoved>()
            .RegisterHandlerForAllEvents(() => new OfferRemovedInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandlerForLocalEvents(() => new OfferRemovedPostgresEventHandler(postgresConnectionFactory));

        builder
            .ForEventType<OfferPriceChanged>()
            .RegisterHandlerForAllEvents(() => new OfferPriceChangedInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandlerForLocalEvents(() => new OfferPriceChangedPostgresEventHandler(postgresConnectionFactory));

        builder
            .ForEventType<OfferStartedTracking>()
            .RegisterHandlerForAllEvents(() => new OfferStartedTrackingInMemoryStoreEventHandler(inMemoryDatabase))
            .RegisterHandlerForLocalEvents(
                () => new OfferStartedTrackingPostgresEventHandler(postgresConnectionFactory));

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
