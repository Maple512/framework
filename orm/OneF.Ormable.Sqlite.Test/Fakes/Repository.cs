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

namespace OneF.Ormable.Fakes;

using System.Threading.Tasks;
using OneF.Ormable.Database;

public class Repository<TEntity> : RepositoryBase<TEntity, SqliteDbContext1>
    where TEntity : class
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public Repository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public override async ValueTask<bool> HasTableAsync(TEntity data)
    {
        await using var connection = await _dbConnectionFactory.CreateAsync();

        await connection.OpenAsync();

        using var command = connection.CreateCommand();

        command.CommandType = System.Data.CommandType.Text;
        command.CommandText = "SELECT COUNT(*) FROM \"sqlite_master\" WHERE \"type\" = 'table' AND \"rootpage\" IS NOT NULL;";

        var result = (long)(await command.ExecuteScalarAsync())!;

        return result != 0;
    }
}
