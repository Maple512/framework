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

namespace OneF.Eventable.Fakes;

using System;
using System.Threading;
using System.Threading.Tasks;
using OneF.Moduleable.DependencyInjection;

public class DelayEvent : EventDataBase
{
    public DelayEvent(long id) : base(id)
    {
    }

    public TimeSpan Delay { get; set; }
}

public class DelayEventHandler : EventHandlerBase<DelayEvent>, ITransientService
{
    public override async Task HandlerAsync(DelayEvent data, CancellationToken cancellationToken = default)
    {
        await Task.Delay(data.Delay);
    }
}
