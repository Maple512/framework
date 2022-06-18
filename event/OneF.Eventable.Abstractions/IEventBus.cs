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

using System.Threading.Tasks;

public interface IEventBus
{
    /// <summary>
    /// 发布事件
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    ValueTask PublishAsync<TEventData>(TEventData data)
        where TEventData : IEventData;

    /// <summary>
    /// 发布事件（并发执行事件处理程序）
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    /// <param name="data"></param>
    /// <param name="parallelOptions"><see cref="Parallel"/> 中的参数</param>
    /// <returns></returns>
    ValueTask PublishAsync<TEventData>(TEventData data, ParallelOptions? parallelOptions = null)
        where TEventData : IEventData;

    /// <summary>
    /// 尝试订阅事件
    /// </summary>
    /// <typeparam name="TEventData"></typeparam>
    /// <returns><see langword="true"/>：订阅成功，<see langword="false"/>：已订阅</returns>
    bool TrySubscribe<TEventData>()
        where TEventData : IEventData;
}
