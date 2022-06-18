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
using System.Threading.Tasks;

public interface IEventHandler<in TEventData> : IEventHandler
    where TEventData : IEventData
{
    int? Order { get; }

    Task HandlerAsync(TEventData data);
}

/// <summary>
/// error: 请不要继承这个接口
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IEventHandler
{
}

public abstract class EventHandlerBase<TEventData> : IEventHandler<TEventData>
    where TEventData : IEventData
{
    public int? Order { get; }

    public abstract Task HandlerAsync(TEventData data);
}
