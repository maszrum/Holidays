namespace Holidays.Eventing.Tests;

internal static class CreateEventBus
{
    public static IEventBus WithRegisteredTestEventHandler(ICollection<TestEvent> eventList)
    {
        var eventBusBuilder = new EventBusBuilder();

        eventBusBuilder
            .ForEventType<TestEvent>()
            .RegisterHandlerForLocalEvents(() => new TestEventFirstHandler(eventList.Add));

        var eventBus = eventBusBuilder.Build();

        return eventBus;
    }

    public static IEventBus WithTwoRegisteredTestEventHandlers(ICollection<TestEvent> eventList)
    {
        var eventBusBuilder = new EventBusBuilder();

        eventBusBuilder
            .ForEventType<TestEvent>()
            .RegisterHandlerForLocalEvents(() => new TestEventFirstHandler(eventList.Add))
            .RegisterHandlerForLocalEvents(() => new TestEventSecondHandler(eventList.Add));

        var eventBus = eventBusBuilder.Build();

        return eventBus;
    }
}
