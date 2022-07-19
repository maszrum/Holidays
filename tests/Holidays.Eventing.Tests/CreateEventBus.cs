namespace Holidays.Eventing.Tests;

internal static class CreateEventBus
{
    public static IEventBus WithRegisteredTestEventHandler(ICollection<TestEvent> eventList)
    {
        var eventBusBuilder = new EventBusBuilder();

        eventBusBuilder
            .ForEventType<TestEvent>()
            .RegisterHandlerForLocalEventsOnly(() => new TestEventFirstHandler(eventList.Add));

        var eventBus = eventBusBuilder.Build();

        return eventBus;
    }

    public static IEventBus WithTwoRegisteredTestEventHandlers(ICollection<TestEvent> eventList)
    {
        var eventBusBuilder = new EventBusBuilder();

        eventBusBuilder
            .ForEventType<TestEvent>()
            .RegisterHandlerForLocalEventsOnly(() => new TestEventFirstHandler(eventList.Add))
            .RegisterHandlerForLocalEventsOnly(() => new TestEventSecondHandler(eventList.Add));

        var eventBus = eventBusBuilder.Build();

        return eventBus;
    }
}
