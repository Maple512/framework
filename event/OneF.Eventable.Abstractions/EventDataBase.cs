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
using System.Diagnostics;
using System.Reflection;

[Serializable]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public abstract class EventDataBase : IEventData
{
    protected EventDataBase(long id)
    {
        Id = id;
        CreatedTime = DateTimeOffset.UtcNow;
    }

    public long Id { get; }

    public DateTimeOffset CreatedTime { get; }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    private string GetDebuggerDisplay()
    {
        return $"Event: {GetType().GetShortDisplayName()}, Id: {Id}, CreatedTime: {CreatedTime:u}";
    }
}
