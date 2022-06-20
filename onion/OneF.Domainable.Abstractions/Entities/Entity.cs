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

namespace OneF.Domainable.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// 实体
/// </summary>
[Serializable]
public class Entity : IEntity
{

}

/// <summary>
/// 实体
/// </summary>
/// <typeparam name="TId"></typeparam>
[Serializable]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Entity<TId> : Entity, IEquatable<Entity<TId>>
    where TId : notnull
{
    protected Entity() { }

    protected Entity(TId id)
    {
        Id = id;
    }

    /// <summary>
    /// 唯一ID
    /// </summary>
    [Key]
    public TId Id { get; set; }

    public bool Equals(Entity<TId>? other)
    {
        return other != null && other.Id.Equals(Id);
    }

    public override bool Equals(object? obj)
    {
        if(obj is Entity<TId> other)
        {
            return Equals(other);
        }

        return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    private string GetDebuggerDisplay()
    {
        return $"{nameof(Entity)}: {GetType().GetShortDisplayName()}, {nameof(Id)}: {Id}";
    }
}
