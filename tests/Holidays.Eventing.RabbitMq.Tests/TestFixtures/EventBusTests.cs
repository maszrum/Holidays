using System.Collections.Concurrent;
using NUnit.Framework;

namespace Holidays.Eventing.RabbitMq.Tests.TestFixtures;

[TestFixture]
public class EventBusTests
{
    [Test]
    public async Task send_two_events_from_one_bus_and_it_should_be_received_by_second_bus_with_correct_data()
    {
        var localEvents = new List<TestEvent>();
        var allEvents = new List<TestEvent>();

        var localEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(localEvents.Add);

        var allEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(allEvents.Add);

        await using var consumerEventBus = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => localEventsHandler)
                .RegisterHandlerForAllEvents(() => allEventsHandler));

        await using var producerEventBus = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .NoLocalHandlers());

        await producerEventBus.Publish(
            new TestEvent(new DateTime(2022, 7, 13, 18, 38, 22), 2202, "qqq"), 
            CancellationToken.None);
        
        await producerEventBus.Publish(
            new TestEvent(new DateTime(2022, 7, 13, 18, 39, 23), 212, "taa"), 
            CancellationToken.None);

        await Wait.Until(
            () => allEvents.Count == 2, 
            TimeSpan.FromSeconds(2));
        
        Assert.That(allEvents, Has.Count.EqualTo(2));
        Assert.That(localEvents, Has.Count.EqualTo(0));
        
        Assert.That(
            allEvents[0].Timestamp, 
            Is.EqualTo(new DateTime(2022, 7, 13, 18, 38, 22)));
        
        Assert.That(
            allEvents[0].IntValue, 
            Is.EqualTo(2202));
        
        Assert.That(
            allEvents[0].StringValue, 
            Is.EqualTo("qqq"));
        
        Assert.That(
            allEvents[1].Timestamp, 
            Is.EqualTo(new DateTime(2022, 7, 13, 18, 39, 23)));
        
        Assert.That(
            allEvents[1].IntValue, 
            Is.EqualTo(212));
        
        Assert.That(
            allEvents[1].StringValue, 
            Is.EqualTo("taa"));
    }
    
    [Test]
    public async Task send_event_from_one_bus_and_it_should_be_received_by_both_buses_with_correct_data()
    {
        var busOneLocalEvents = new List<TestEvent>();
        var busOneAllEvents = new List<TestEvent>();
        var busTwoLocalEvents = new List<TestEvent>();
        var busTwoAllEvents = new List<TestEvent>();

        var busOneLocalEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busOneLocalEvents.Add);

        var busOneAllEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busOneAllEvents.Add);

        var busTwoLocalEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busTwoLocalEvents.Add);

        var busTwoAllEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busTwoAllEvents.Add);

        await using var eventBusOne = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => busOneLocalEventsHandler)
                .RegisterHandlerForAllEvents(() => busOneAllEventsHandler));

        await using var eventBusTwo = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => busTwoLocalEventsHandler)
                .RegisterHandlerForAllEvents(() => busTwoAllEventsHandler));

        await eventBusOne.Publish(
            new TestEvent(new DateTime(2022, 7, 13, 19, 2, 22), 111, "abc"), 
            CancellationToken.None);

        await Wait.Until(
            () => busOneLocalEvents.Count == 1 && busOneAllEvents.Count == 1 && busTwoLocalEvents.Count == 0 && busTwoAllEvents.Count == 1, 
            TimeSpan.FromSeconds(2));
        
        Assert.That(busOneLocalEvents, Has.Count.EqualTo(1));
        Assert.That(busOneAllEvents, Has.Count.EqualTo(1));
        Assert.That(busTwoLocalEvents, Has.Count.EqualTo(0));
        Assert.That(busTwoAllEvents, Has.Count.EqualTo(1));
        
        Assert.That(
            busOneLocalEvents[0].Timestamp, 
            Is.EqualTo(new DateTime(2022, 7, 13, 19, 2, 22)));
        
        Assert.That(
            busOneLocalEvents[0].IntValue, 
            Is.EqualTo(111));
        
        Assert.That(
            busOneLocalEvents[0].StringValue, 
            Is.EqualTo("abc"));
        
        Assert.That(
            busOneAllEvents[0].Timestamp, 
            Is.EqualTo(new DateTime(2022, 7, 13, 19, 2, 22)));
        
        Assert.That(
            busOneAllEvents[0].IntValue, 
            Is.EqualTo(111));
        
        Assert.That(
            busOneAllEvents[0].StringValue, 
            Is.EqualTo("abc"));
        
        Assert.That(
            busTwoAllEvents[0].Timestamp, 
            Is.EqualTo(new DateTime(2022, 7, 13, 19, 2, 22)));
        
        Assert.That(
            busTwoAllEvents[0].IntValue, 
            Is.EqualTo(111));
        
        Assert.That(
            busTwoAllEvents[0].StringValue, 
            Is.EqualTo("abc"));
    }
    
    [Test]
    public async Task send_two_events_from_one_bus_and_it_should_be_received_by_both_buses()
    {
        var busOneLocalEvents = new List<TestEvent>();
        var busOneAllEvents = new List<TestEvent>();
        var busTwoLocalEvents = new List<TestEvent>();
        var busTwoAllEvents = new List<TestEvent>();

        var busOneLocalEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busOneLocalEvents.Add);

        var busOneAllEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busOneAllEvents.Add);

        var busTwoLocalEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busTwoLocalEvents.Add);

        var busTwoAllEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busTwoAllEvents.Add);

        await using var eventBusOne = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => busOneLocalEventsHandler)
                .RegisterHandlerForAllEvents(() => busOneAllEventsHandler));

        await using var eventBusTwo = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => busTwoLocalEventsHandler)
                .RegisterHandlerForAllEvents(() => busTwoAllEventsHandler));

        await eventBusOne.Publish(
            new TestEvent(new DateTime(2022, 7, 13, 18, 38, 22), 2202, "qqq"), 
            CancellationToken.None);
        
        await eventBusOne.Publish(
            new TestEvent(new DateTime(2022, 7, 13, 18, 39, 23), 212, "taa"), 
            CancellationToken.None);

        await Wait.Until(
            () => busOneLocalEvents.Count == 2 && busOneAllEvents.Count == 2 && busTwoLocalEvents.Count == 0 && busTwoAllEvents.Count == 2, 
            TimeSpan.FromSeconds(2));
        
        Assert.That(busOneLocalEvents, Has.Count.EqualTo(2));
        Assert.That(busOneAllEvents, Has.Count.EqualTo(2));
        Assert.That(busTwoLocalEvents, Has.Count.EqualTo(0));
        Assert.That(busTwoAllEvents, Has.Count.EqualTo(2));
    }
    
    [Test]
    public async Task send_many_events_from_both_buses_concurrently_and_every_should_arrive()
    {
        var busOneLocalEvents = new ConcurrentBag<TestEvent>();
        var busOneAllEvents = new ConcurrentBag<TestEvent>();
        var busTwoLocalEvents = new ConcurrentBag<TestEvent>();
        var busTwoAllEvents = new ConcurrentBag<TestEvent>();

        var busOneLocalEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busOneLocalEvents.Add);

        var busOneAllEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busOneAllEvents.Add);

        var busTwoLocalEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busTwoLocalEvents.Add);

        var busTwoAllEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busTwoAllEvents.Add);

        await using var eventBusOne = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => busOneLocalEventsHandler)
                .RegisterHandlerForAllEvents(() => busOneAllEventsHandler));

        await using var eventBusTwo = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => busTwoLocalEventsHandler)
                .RegisterHandlerForAllEvents(() => busTwoAllEventsHandler));

        var events = Enumerable
            .Range(1, 20000)
            .Select(i => new TestEvent(new DateTime(), i, $"event-{i}"))
            .ToArray();

        var eventsFirstPart = events.Take(10000);
        var eventsSecondPart = events.Skip(10000).Take(10000);

        // ReSharper disable once AccessToDisposedClosure
        var firstPublishTask = Parallel.ForEachAsync(
            eventsFirstPart, 
            async (e, ct) => await eventBusOne.Publish(e, ct));

        // ReSharper disable once AccessToDisposedClosure
        var secondPublishTask = Parallel.ForEachAsync(
            eventsSecondPart, 
            async (e, ct) => await eventBusTwo.Publish(e, ct));

        await Task.WhenAll(firstPublishTask, secondPublishTask);

        await Wait.Until(
            () => busOneLocalEvents.Count == 10000 && busOneAllEvents.Count == 20000 && 
                  busTwoLocalEvents.Count == 10000 && busTwoAllEvents.Count == 20000, 
            TimeSpan.FromSeconds(5));
        
        Assert.That(busOneLocalEvents, Has.Count.EqualTo(10000));
        Assert.That(busOneAllEvents, Has.Count.EqualTo(20000));
        Assert.That(busTwoLocalEvents, Has.Count.EqualTo(10000));
        Assert.That(busTwoAllEvents, Has.Count.EqualTo(20000));
        
        CollectionAssert.AreEquivalent(
            Enumerable.Range(1, 10000), 
            busOneLocalEvents.Select(e => e.IntValue));
        
        CollectionAssert.AreEquivalent(
            Enumerable.Range(1, 20000), 
            busOneAllEvents.Select(e => e.IntValue));
        
        CollectionAssert.AreEquivalent(
            Enumerable.Range(10001, 10000), 
            busTwoLocalEvents.Select(e => e.IntValue));
        
        CollectionAssert.AreEquivalent(
            Enumerable.Range(1, 20000), 
            busTwoAllEvents.Select(e => e.IntValue));
    }
    
    [Test]
    public async Task send_event_from_one_bus_and_it_should_be_received_by_three_buses()
    {
        var busOneLocalEvents = new List<TestEvent>();
        var busOneAllEvents = new List<TestEvent>();
        var busTwoLocalEvents = new List<TestEvent>();
        var busTwoAllEvents = new List<TestEvent>();
        var busThreeLocalEvents = new List<TestEvent>();
        var busThreeAllEvents = new List<TestEvent>();

        var busOneLocalEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busOneLocalEvents.Add);

        var busOneAllEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busOneAllEvents.Add);

        var busTwoLocalEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busTwoLocalEvents.Add);

        var busTwoAllEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busTwoAllEvents.Add);

        var busThreeLocalEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busThreeLocalEvents.Add);

        var busThreeAllEventsHandler = new TestEventHandler<TestEvent>()
            .DoOnEvent(busThreeAllEvents.Add);

        await using var eventBusOne = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => busOneLocalEventsHandler)
                .RegisterHandlerForAllEvents(() => busOneAllEventsHandler));

        await using var eventBusTwo = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => busTwoLocalEventsHandler)
                .RegisterHandlerForAllEvents(() => busTwoAllEventsHandler));

        await using var eventBusThree = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .RegisterHandlerForLocalEvents(() => busThreeLocalEventsHandler)
                .RegisterHandlerForAllEvents(() => busThreeAllEventsHandler));

        await eventBusOne.Publish(
            new TestEvent(new DateTime(2022, 7, 13, 20, 20, 22), 111, "bbb"), 
            CancellationToken.None);

        await Wait.Until(
            () => busOneLocalEvents.Count == 1 && busOneAllEvents.Count == 1 && 
                  busTwoLocalEvents.Count == 0 && busTwoAllEvents.Count == 1 && 
                  busThreeLocalEvents.Count == 0 && busThreeAllEvents.Count == 1, 
            TimeSpan.FromSeconds(2));
        
        Assert.That(busOneLocalEvents, Has.Count.EqualTo(1));
        Assert.That(busOneAllEvents, Has.Count.EqualTo(1));
        Assert.That(busTwoLocalEvents, Has.Count.EqualTo(0));
        Assert.That(busTwoAllEvents, Has.Count.EqualTo(1));
        Assert.That(busThreeLocalEvents, Has.Count.EqualTo(0));
        Assert.That(busThreeAllEvents, Has.Count.EqualTo(1));
    }
    
    [Test]
    public async Task send_event_from_one_bus_and_it_and_error_action_should_be_invoked_on_second_bus()
    {
        var unknownTypes = new List<string>();
        
        await using var consumerEventBus = await Create.EventBus(_ => { }, unknownTypes.Add);

        await using var producerEventBus = await Create.EventBus(
            builder => builder
                .ForEventType<TestEvent>()
                .NoLocalHandlers());

        await producerEventBus.Publish(
            new TestEvent(new DateTime(2022, 7, 13, 18, 38, 22), 2202, "qqq"), 
            CancellationToken.None);

        await Wait.Until(
            () => unknownTypes.Count == 1, 
            TimeSpan.FromSeconds(1));
        
        Assert.That(unknownTypes, Has.Count.EqualTo(1));
        Assert.That(unknownTypes[0], Is.EqualTo(nameof(TestEvent)));
    }
}
