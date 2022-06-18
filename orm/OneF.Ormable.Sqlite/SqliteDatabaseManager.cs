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

namespace OneF.Ormable;

using System;
using System.Threading;
using System.Threading.Tasks;
using OneF.Ormable.Database;

public sealed class SqliteDatabaseManager<TDbContext> : DatabaseManager<TDbContext>
    where TDbContext : class, IDatabase
{
    public override ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override ValueTask EnsureCreateTablesAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<bool> ExistsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override ValueTask<string> GenerateCreateScript(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
