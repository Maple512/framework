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

namespace OneF.Domainable;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

[Serializable]
public abstract class Entity : IEntity
{

}

[Serializable]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Entity<TKey> : Entity, IEqualityComparer<Entity<TKey>>, IEquatable<Entity<TKey>>
    where TKey : notnull
{
    public TKey Key { get; set; }

    public bool Equals(Entity<TKey>? x, Entity<TKey>? y)
    {
        if(x == null
            && y == null)
        {
            return true;
        }

        if(x == null
            || y == null)
        {
            return false;
        }

        return x.Key.Equals(y.Key);
    }

    public bool Equals(Entity<TKey>? other)
    {
        return other != null && other.Key.Equals(Key);
    }

    public int GetHashCode([DisallowNull] Entity<TKey> obj)
    {
        return obj.Key.GetHashCode();
    }

    public override string ToString()
    {
        return $"Entity: {Key}";
    }

    private string GetDebuggerDisplay()
    {
        return GetType().GetShortDisplayName();
    }
}
