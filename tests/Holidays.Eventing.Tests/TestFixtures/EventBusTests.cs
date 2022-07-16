using Holidays.Core.Eventing;
using Moq;
using NUnit.Framework;

namespace Holidays.Eventing.Tests.TestFixtures;

[TestFixture]
public class EventBusTests
{
    [Test]
    public async Task register_one_handler_and_published_event_should_arrive()
    {
        var handledEvents = new List<TestEvent>();

        var eventBus = CreateEventBus.WithRegisteredTestEventHandler(handledEvents);

        var @event = new TestEvent();
        await eventBus.Publish(@event);

        Assert.That(handledEvents, Has.Count.EqualTo(1));
        Assert.That(ReferenceEquals(handledEvents[0], @event), Is.True);
    }

    [Test]
    public async Task register_one_handler_and_published_many_events_concurrently_should_arrive()
    {
        var handledEvents = new List<TestEvent>();

        var eventBus = CreateEventBus.WithRegisteredTestEventHandler(handledEvents);

        var events = Enumerable
            .Repeat(0, 20)
            .Select(_ => new TestEvent())
            .ToArray();

        var tasks = events
            .Select(e => eventBus.Publish(e))
            .ToArray();

        await Task.WhenAll(tasks);

        Assert.That(handledEvents, Has.Count.EqualTo(20));
        CollectionAssert.AreEquivalent(events, handledEvents);
    }

    [Test]
    public async Task register_two_handler_and_published_event_should_arrive_in_correct_order()
    {
        var handledEvents = new List<TestEvent>();

        var eventBus = CreateEventBus.WithTwoRegisteredTestEventHandlers(handledEvents);

        var @event = new TestEvent();
        await eventBus.Publish(@event);

        Assert.That(handledEvents, Has.Count.EqualTo(2));
        Assert.That(ReferenceEquals(handledEvents[0], @event), Is.True);
        Assert.That(ReferenceEquals(handledEvents[1], @event), Is.True);
    }

    [Test]
    public void sending_event_that_was_not_registered_should_throw_exception()
    {
        var eventBus = new EventBusBuilder().Build();

        Assert.ThrowsAsync<InvalidOperationException>(
            () => eventBus.Publish(new TestEvent()));
    }

    [Test]
    public void second_handler_should_throw_so_first_one_should_roll_back()
    {
        var eventBusBuilder = new EventBusBuilder();

        var firstHandler = new TestEventFirstHandler(_ => { });
        var secondHandler = new TestEventSecondHandler(_ => throw new InvalidOperationException());

        eventBusBuilder
            .ForEventType<TestEvent>()
            .RegisterHandlerForLocalEvents(() => firstHandler)
            .RegisterHandlerForLocalEvents(() => secondHandler);

        var eventBus = eventBusBuilder.Build();

        Assert.ThrowsAsync<InvalidOperationException>(
            () => eventBus.Publish(new TestEvent()));

        Assert.That(firstHandler.RolledBack, Is.True);
        Assert.That(firstHandler.Committed, Is.False);
        Assert.That(secondHandler.RolledBack, Is.True);
        Assert.That(secondHandler.Committed, Is.False);
    }

    [Test]
    public async Task none_of_handlers_throw_so_every_should_have_committed_property_true()
    {
        var eventBusBuilder = new EventBusBuilder();

        var firstHandler = new TestEventFirstHandler(_ => { });
        var secondHandler = new TestEventSecondHandler(_ => { });

        eventBusBuilder
            .ForEventType<TestEvent>()
            .RegisterHandlerForLocalEvents(() => firstHandler)
            .RegisterHandlerForLocalEvents(() => secondHandler);

        var eventBus = eventBusBuilder.Build();

        await eventBus.Publish(new TestEvent());

        Assert.That(firstHandler.RolledBack, Is.False);
        Assert.That(firstHandler.Committed, Is.True);
        Assert.That(secondHandler.RolledBack, Is.False);
        Assert.That(secondHandler.Committed, Is.True);
    }

    [Test]
    public async Task when_external_provider_fails_on_publishing_event_it_should_rollback_every_handler()
    {
        var externalProvider = new Mock<IExternalProvider>();
        var eventSink = new Mock<IExternalEventSink>();

        eventSink
            .Setup(m => m.Publish(It.IsAny<IEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromException(new ArithmeticException()));

        externalProvider
            .SetupGet(m => m.Sink)
            .Returns(eventSink.Object);

        externalProvider
            .Setup(m => m.Initialize(It.IsAny<EventBus>()))
            .Returns(Task.CompletedTask);

        var eventBusBuilder = new EventBusBuilder()
            .UseExternalProvider(externalProvider.Object);

        var firstHandler = new TestEventFirstHandler(_ => { });
        var secondHandler = new TestEventSecondHandler(_ => { });

        eventBusBuilder
            .ForEventType<TestEvent>()
            .RegisterHandlerForAllEvents(() => firstHandler)
            .RegisterHandlerForLocalEvents(() => secondHandler);

        var eventBus = await eventBusBuilder.Build().Initialize();

        var @event = new TestEvent();

        try
        {
            await eventBus.Publish(@event, CancellationToken.None);
        }
        catch (ArithmeticException)
        {
            // ignored
        }

        Assert.That(firstHandler.Committed, Is.False);
        Assert.That(firstHandler.RolledBack, Is.True);
        Assert.That(secondHandler.Committed, Is.False);
        Assert.That(secondHandler.RolledBack, Is.True);
    }
}
