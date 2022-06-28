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

using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

public interface IEventHandler<in TEventData> : IEventHandler
    where TEventData : IEventData
{
    Task HandlerAsync(TEventData data, CancellationToken cancellationToken = default);
}

public interface IEventHandler<in TEventData, TEventResult> : IEventHandler
{
    Task<TEventResult> HandlerAsync(TEventData eventData, CancellationToken cancellationToken = default);
}

/// <summary>
/// 事件处理器
/// <para>注意：请不要继承这个接口！！！，请使用<see cref="IEventHandler{TEventData}"/>泛型接口</para>
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IEventHandler
{
    int? Order { get; }
}

public abstract class EventHandlerBase<TEventData> : IEventHandler<TEventData>
    where TEventData : IEventData
{
    public int? Order { get; }

    public abstract Task HandlerAsync(TEventData data, CancellationToken cancellationToken = default);
}

public abstract class EventHandlerBase<TEventData, TEventResult> : IEventHandler<TEventData, TEventResult>
    where TEventData : IEventData
{
    public int? Order { get; }

    public abstract Task<TEventResult> HandlerAsync(TEventData data, CancellationToken cancellationToken = default);
}
