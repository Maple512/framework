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

using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// 数据库连接
/// TODO: 暂时不加事务
/// </summary>
public interface IDatabaseConnection : /*IDatabaseTransactionManager,*/ IDisposable, IAsyncDisposable
{
    Guid Id { get; }

    string? ConnectionString { get; }

    [AllowNull]
    DbConnection Connection { get; }

    /// <summary>
    /// 数据库执行命令的超时时间
    /// </summary>
    int? CommandTimeout { get; }

    /// <summary>
    /// 打开数据库连接
    /// </summary>
    /// <returns>实际已经打开连接<see langword="true"/>，否则<see langword="false"/></returns>
    bool Open();

    /// <summary>
    /// 打开数据库连接
    /// </summary>
    /// <returns>实际已经打开连接<see langword="true"/>，否则<see langword="false"/></returns>
    Task<bool> OpenAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    /// <returns>实际已经关闭连接<see langword="true"/>，否则<see langword="false"/></returns>
    bool Close();

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    /// <returns>实际已经关闭连接<see langword="true"/>，否则<see langword="false"/></returns>
    Task<bool> CloseAsync();
}
