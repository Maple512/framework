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

using System.Linq;
using System.Threading.Tasks;
using OneF.Domainable.Entities;

public interface IBaseRepository<TEntity>
    where TEntity : class, IEntity
{
    Task<IQueryable<TEntity>> AsQueryableAsync();
}
