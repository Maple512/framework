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

namespace OneF.Ormable.Database;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 数据库管理器
/// </summary>
public interface IDatabaseManager<TDbContext>
    where TDbContext : class, IDatabase
{
    /// <summary>
    /// 数据库是否已存在
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns><see langword="true"/>：数据库已存在，<see langword="false"/>：数据库不存在</returns>
    ValueTask<bool> ExistsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 确保数据库已被删除
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns><see langword="true"/>：数据库已被删除，<see langword="false"/>：数据库不存在</returns>
    ValueTask<bool> EnsureDeletedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 确保数据库已被创建
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns><see langword="true"/>：数据库已被创建，<see langword="false"/>：数据库已存在</returns>
    ValueTask<bool> EnsureCreatedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 确保所有表都已经创建
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask EnsureCreateTablesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 生成创建数据库的脚本
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    ValueTask<string> GenerateCreateScript(CancellationToken cancellationToken = default);
}
