using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Haus.Cqrs;

internal static class HandlerInvoker
{
    public static Task InvokeAsync(IServiceProvider services, Type handlerType, object message, CancellationToken token)
    {
        var handler = ResolveSingleHandler(services, handlerType);
        return (Task)Invoke(handlerType, handler, message, token);
    }

    public static Task<TResult> InvokeAsync<TResult>(
        IServiceProvider services,
        Type handlerType,
        object message,
        CancellationToken token
    )
    {
        var handler = ResolveSingleHandler(services, handlerType);
        return (Task<TResult>)Invoke(handlerType, handler, message, token);
    }

    public static async Task InvokeAllAsync(
        IServiceProvider services,
        Type handlerType,
        object message,
        CancellationToken token
    )
    {
        foreach (var handler in services.GetServices(handlerType))
        {
            if (handler is null)
                throw new InvalidOperationException($"A null handler was registered for {handlerType}.");

            await ((Task)Invoke(handlerType, handler, message, token)).ConfigureAwait(false);
        }
    }

    private static object Invoke(Type handlerType, object handler, object message, CancellationToken token)
    {
        var handleMethod = handlerType.GetMethod("Handle");
        if (handleMethod is null)
            throw new InvalidOperationException($"{handlerType} does not declare a Handle method.");

        object? result;
        try
        {
            result = handleMethod.Invoke(handler, [message, token]);
        }
        catch (TargetInvocationException e) when (e.InnerException is not null)
        {
            // Throw() always throws; this satisfies the compiler's control-flow analysis.
            ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }

        if (result is null)
            throw new InvalidOperationException($"{handlerType}.Handle returned null instead of a Task.");

        return result;
    }

    private static object ResolveSingleHandler(IServiceProvider services, Type handlerType)
    {
        var handlers = services.GetServices(handlerType).ToList();
        return handlers.Count switch
        {
            0 => throw new InvalidOperationException($"No handler registered for {handlerType}."),
            1 => handlers[0]
                ?? throw new InvalidOperationException($"A null handler was registered for {handlerType}."),
            _ => throw new InvalidOperationException($"Multiple handlers registered for {handlerType}."),
        };
    }
}
