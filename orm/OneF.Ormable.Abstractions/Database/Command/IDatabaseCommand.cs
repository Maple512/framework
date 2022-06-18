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

namespace OneF.Ormable.Database.Command;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 数据库命令
/// </summary>
public interface IDatabaseCommand : IDatabaseCommandInfo
{
    int ExecuteNonQuery();

    Task<int> ExecuteNonQuery(CancellationToken cancellationToken = default);

    object ExecuteScalar();

    Task<object> ExecuteScalarAsync(CancellationToken cancellationToken = default);

    T ExecuteScalar<T>();

    Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken = default);

    IDatabaseReader ExecuteReader();

    Task<IDatabaseReader> ExecuteReaderAsync(CancellationToken cancellationToken = default);
}
