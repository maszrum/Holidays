namespace Holidays.Eventing.Tests;

internal static class CreateEventBus
{
    public static EventBus WithRegisteredTestEventHandler(ICollection<TestEvent> eventList)
    {
        var eventBusBuilder = new EventBusBuilder();

        eventBusBuilder
            .ForEventType<TestEvent>()
            .RegisterHandler(() => new TestEventFirstHandler(eventList.Add));

        return eventBusBuilder.Build();
    }

    public static EventBus WithTwoRegisteredTestEventHandlers(ICollection<TestEvent> eventList)
    {
        var eventBusBuilder = new EventBusBuilder();

        eventBusBuilder
            .ForEventType<TestEvent>()
            .RegisterHandler(() => new TestEventFirstHandler(eventList.Add))
            .RegisterHandler(() => new TestEventSecondHandler(eventList.Add));

        return eventBusBuilder.Build();
    }
}
