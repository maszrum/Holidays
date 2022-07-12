using System.Collections.Immutable;
using System.Reflection;
using Holidays.Core.Eventing;

namespace Holidays.Eventing;

public class EventBus
{
    private readonly IReadOnlyDictionary<Type, List<Func<object>>> _handlerFactories;
    private readonly ImmutableDictionary<Type, MethodInfo> _methodInfos;

    internal EventBus(IReadOnlyDictionary<Type, List<Func<object>>> handlerFactories)
    {
        _handlerFactories = handlerFactories;

        foreach (var (_, handlerFactory) in handlerFactories)
        {
            handlerFactory.Reverse();
        }

        _methodInfos = GetMethodInfos(handlerFactories.Values.SelectMany(h => h));
    }

    public async Task Publish(IEvent @event, CancellationToken cancellationToken = default)
    {
        var eventType = @event.GetType();
        
        if (!_handlerFactories.TryGetValue(eventType, out var factories))
        {
            throw new InvalidOperationException(
                $"Specified event type has not been registered when building {nameof(EventBus)}.");
        }

        var next = () => Task.CompletedTask;

        foreach (var handlerFactory in factories)
        {
            var nextHandler = next;
            var nextNext = () => InvokeHandleMethod(handlerFactory, @event, nextHandler, cancellationToken);
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
