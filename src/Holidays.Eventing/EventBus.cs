using System.Collections.Immutable;
using System.Reflection;
using Holidays.Core.Eventing;

namespace Holidays.Eventing;

public sealed class EventBus : IEventBus
{
    private readonly IReadOnlyDictionary<Type, List<EventHandlerDescriptor>> _handlerFactories;
    private readonly ImmutableDictionary<Type, MethodInfo> _methodInfos;
    private readonly IReadOnlyList<IExternalProvider> _externalProviders;

    internal EventBus(
        IReadOnlyDictionary<Type, List<EventHandlerDescriptor>> handlerFactories,
        IReadOnlyList<IExternalProvider> externalProviders)
    {
        _handlerFactories = handlerFactories;
        _externalProviders = externalProviders;

        foreach (var (_, handlerFactory) in handlerFactories)
        {
            handlerFactory.Reverse();
        }

        var factories = handlerFactories.Values
            .SelectMany(hl => hl.Select(h => h.HandlerFactory));
        _methodInfos = GetMethodInfos(factories);
    }

    public Task Publish(
        IEvent @event,
        CancellationToken cancellationToken = default)
    {
        return PublishEvent(
            @event, 
            asExternalProvider: false, 
            cancellationToken);
    }

    public Task PublishAsExternalProvider(
        IEvent @event,
        CancellationToken cancellationToken = default)
    {
        return PublishEvent(
            @event, 
            asExternalProvider: true, 
            cancellationToken);
    }

    public IEnumerable<Type> GetRegisteredEventTypes() => _handlerFactories.Keys;

    public async ValueTask DisposeAsync()
    {
        foreach (var externalProvider in _externalProviders)
        {
            await externalProvider.DisposeAsync();
        }
    }

    private async Task PublishEvent(
        IEvent @event,
        bool asExternalProvider,
        CancellationToken cancellationToken)
    {
        var eventType = @event.GetType();
        
        if (!_handlerFactories.TryGetValue(eventType, out var descriptors))
        {
            throw new InvalidOperationException(
                $"Specified event type has not been registered when building {nameof(EventBus)}.");
        }

        var descriptorsToHandle = asExternalProvider
            ? descriptors.Where(d => !d.OnlyForLocalEvents)
            : descriptors;

        Func<Task> next = asExternalProvider
            ? () => Task.CompletedTask
            : () => PublishToExternalProviders(@event, cancellationToken);

        foreach (var descriptor in descriptorsToHandle)
        {
            var nextHandler = next;
            
            var nextNext = () => 
                InvokeHandleMethod(descriptor.HandlerFactory, @event, nextHandler, cancellationToken);
            
            next = nextNext;
        }

        await next();
    }

    private async Task InvokeHandleMethod(
        Func<object> handlerFactory, 
        IEvent @event, 
        Func<Task> next,
        CancellationToken cancellationToken = default)
    {
        var handlerInstance = handlerFactory();
        var handleMethodInfo = _methodInfos[handlerInstance.GetType()];
        
        var result = handleMethodInfo.Invoke(
            handlerInstance, 
            new object[] { @event, next, cancellationToken });

        if (result is null)
        {
            throw new InvalidOperationException(
                $"Method {nameof(IEventHandler<IEvent>.Handle)} returned null value");
        }

        var resultTyped = (Task) result;

        await resultTyped;
    }

    private async Task PublishToExternalProviders(IEvent @event, CancellationToken cancellationToken)
    {
        foreach (var externalProvider in _externalProviders)
        {
            await externalProvider.Sink.Publish(@event, cancellationToken);
        }
    }

    private static ImmutableDictionary<Type, MethodInfo> GetMethodInfos(IEnumerable<Func<object>> handlerFactories)
    {
        var result = ImmutableDictionary.CreateBuilder<Type, MethodInfo>();

        foreach (var handlerFactory in handlerFactories)
        {
            var handlerInstance = handlerFactory();
            var handlerType = handlerInstance.GetType();

            if (result.ContainsKey(handlerType))
            {
                continue;
            }

            var handleMethodInfo = handlerInstance
                .GetType()
                .GetMethod(nameof(IEventHandler<IEvent>.Handle));

            if (handleMethodInfo is null)
            {
                throw new InvalidOperationException(
                    $"Cannot find {nameof(IEventHandler<IEvent>.Handle)} method " +
                    $"in type {handlerInstance.GetType().Name}");
            }
            
            result.Add(handlerType, handleMethodInfo);
        }

        return result.ToImmutable();
    }
}
