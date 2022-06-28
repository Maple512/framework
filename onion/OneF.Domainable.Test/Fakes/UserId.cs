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

namespace OneF.Domainable.Fakes;

using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OneF.Domainable.Entities;

public readonly record struct UserId(long Key) : IUserId
{
    public static implicit operator long(UserId user)
    {
        return user.Key;
    }

    public static implicit operator UserId(long id)
    {
        return new UserId(id);
    }

    public override string ToString()
    {
        return $"{Key}";
    }
}

public class UserIdConverter : ValueConverter<IUserId, long>
{
    public UserIdConverter(ConverterMappingHints? mappingHints = null)
        : base(ToLong(), ToUserId(), mappingHints)
    {

    }

    private static Expression<Func<IUserId, long>> ToLong()
    {
        return static (IUserId id) => (long)(UserId)id;
    }

    private static Expression<Func<long, IUserId>> ToUserId()
    {
        return static (long id) => (UserId)id;
    }
}
