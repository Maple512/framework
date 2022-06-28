// Copyright 2021 Maple512 and Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License"),
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace OneF.Eventable;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OneF.Moduleable.DependencyInjection;

public sealed class EventBus : IEventBus, ISingletonService
{
    public EventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _cache = new();
    }

    private readonly IServiceProvider _serviceProvider;

    private readonly ConcurrentDictionary<string, IEnumerable<IEventHandler>> _cache;

    public bool TrySubscribe<TEventData>() where TEventData : IEventData
    {
        var eventType = typeof(TEventData);

        var key = CalculationKey(eventType);

        return _cache.TryAdd(key, FindEventHandler(eventType));
    }

    public async ValueTask PublishAsync<TEventData>(TEventData eventData, CancellationToken cancellationToken = default)
        where TEventData : IEventData
    {
        _ = Check.NotNull(eventData);

        var eventType = typeof(TEventData);

        var key = CalculationKey(eventType);

        var handlers = _cache.GetOrAdd(key, () => FindEventHandler(eventType))
            .OfType<IEventHandler<TEventData>>().OrderBy(x => x.Order);

        foreach(var handler in handlers)
        {
            await handler!.HandlerAsync(eventData, cancellationToken);

            if(handler is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if(handler is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
        }
    }

    public async IAsyncEnumerable<TEventResult> PublishAsync<TEventData, TEventResult>(TEventData eventData, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TEventData : IEventData
    {
        _ = Check.NotNull(eventData);

        var eventType = typeof(TEventData);

        var evnetResultType = typeof(TEventResult);

        var key = CalculationKey(eventType, evnetResultType);

        var handlers = _cache.GetOrAdd(key, () => FindEventHandler(eventType, evnetResultType))
            .OfType<IEventHandler<TEventData, TEventResult>>()
            .OrderBy(x => x.Order);

        foreach(var handler in handlers)
        {
            var result = await handler!.HandlerAsync(eventData, cancellationToken);

            if(handler is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if(handler is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }

            yield return result;
        }
    }

    private IEnumerable<IEventHandler> FindEventHandler(Type eventType)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

        var handlers = _serviceProvider.GetServices<IEventHandler>()
            .Where(x => x.GetType().IsAssignableToType(handlerType));

        if(handlers.IsNullOrEmpty())
        {
            throw new InvalidOperationException($"Could not found event handler: {handlerType.AssemblyQualifiedName}");
        }

        return handlers!;
    }

    private IEnumerable<IEventHandler> FindEventHandler(Type eventType, Type eventResult)
    {
        var handlerType = typeof(IEventHandler<,>).MakeGenericType(eventType, eventResult);

        var handlers = _serviceProvider.GetServices<IEventHandler>()
            .Where(x => x.GetType().IsAssignableToType(handlerType));

        if(handlers.IsNullOrEmpty())
        {
            throw new InvalidOperationException($"Could not found event handler: {handlerType.AssemblyQualifiedName}");
        }

        return handlers!;
    }

    private static string CalculationKey(Type eventData, Type? eventResult = null)
        => $"{eventData.Name}{eventResult?.Name}";
}
