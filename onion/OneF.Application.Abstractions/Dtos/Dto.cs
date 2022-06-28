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

namespace OneF.Application.Dtos;

using System;
using System.Reflection;

public interface IDto { }

public interface IDto<TId> : IDto
    where TId : notnull
{
    TId Id { get; init; }
}

[Serializable]
public class Dto : IDto
{
    public override string ToString()
    {
        return $"DTO: {GetType().GetShortDisplayName()}";
    }
}

[Serializable]
public class Dto<TId> : Dto, IDto<TId>
    where TId : notnull
{
    public TId Id { get; init; }

    public override string ToString()
    {
        return $"DTO: {GetType().GetShortDisplayName()}, {nameof(Id)}: {Id}";
    }
}
