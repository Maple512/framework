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

namespace OneF.Domainable.Repositories;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using OneF.Domainable.Entities;

public interface IReadOnlyRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class, IEntity
{
    /// <summary>
    /// 获取一个实体
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取一个实体集合
    /// </summary>
    /// <param name="predicate">条件</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取实体的数量
    /// </summary>
    /// <param name="predicate">条件</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> GetCountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 分页
    /// </summary>
    /// <param name="offset">偏移量</param>
    /// <param name="count">返回的最大数量</param>
    /// <param name="order">排序规则</param>
    /// <param name="predicate">条件</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> GetPagedAsync(
        int offset,
        int count,
        string? order = null,
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}

public interface IReadOnlyRepository<TEntity, TId> : IReadOnlyRepository<TEntity>
    where TId : notnull
    where TEntity : class, IEntity
{
    /// <summary>
    /// 获取一个实体
    /// </summary>
    /// <param name="id">实体所属的ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity> GetAsync(TId id, CancellationToken cancellationToken = default);
}
