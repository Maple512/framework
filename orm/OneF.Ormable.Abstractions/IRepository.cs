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

using System.Threading.Tasks;
using OneF.Ormable.Database;

/*操控实体的仓储（增删改查）*/
public interface IRepository<TEntity, TDbContext>
    where TEntity : class
    where TDbContext : class, IDatabase
{
    ValueTask<bool> HasTableAsync(TEntity data);
}
