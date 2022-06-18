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

namespace OneF.Ormable.Test;

using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Shouldly;
using Xunit;

public class DbCommon_Test
{
    [Fact]
    public async Task Create_table()
    {
        var connectionString = "Data Source=:memory:";

        using var connection = new SqliteConnection(connectionString);

        await connection.OpenAsync();

        using(var command = connection.CreateCommand())
        {
            command.CommandType = CommandType.Text;
            command.CommandText = @"create table if not exists user([Id] integer not null primary key autoincrement, [Name] nvarchar(64) not null)";

            var parameter = new SqliteParameter
            {
                ParameterName = "aaa",
                Value = 12.5m,
            };

            _ = command.Parameters.Add(parameter);

            var result = await command.ExecuteNonQueryAsync();

            result.ShouldBe(0);
        }

        using(var command1 = connection.CreateCommand())
        {
            command1.CommandType = CommandType.Text;
            command1.CommandText = @"insert into user(Name) values('Maple512')";

            var result = await command1.ExecuteNonQueryAsync();

            result.ShouldBe(1);
        }

        using(var command2 = connection.CreateCommand())
        {
            command2.CommandType = CommandType.Text;
            command2.CommandText = @"select count(1) from user";

            var result = await command2.ExecuteScalarAsync();

            result.ShouldBe(1);

            var reader = await command2.ExecuteReaderAsync();

            _ = reader.Read();

            for(var i = 0; i < reader.FieldCount; i++)
            {
                var type = reader.GetDataTypeName(i);

                var column = reader.GetName(i);

                var i32 = reader.GetInt32(i);

                i32.ShouldBe(1);
            }
        }
    }

    [Fact]
    public async Task Create_Sqlite_DbAsync()
    {
        await Should.NotThrowAsync(EnsureCreateDatabase("data source=./db_common_test.db"));
    }

    [Fact]
    public async Task Has_Tables()
    {
        var connectionString = "data source=./db_common_test.db";

        await EnsureCreateDatabase(connectionString);

        using var connection = new SqliteConnection(connectionString);

        await connection.OpenAsync();

        using var command = connection.CreateCommand();

        command.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type = 'table' AND rootpage IS NOT NULL;";

        var count = (long?)await command.ExecuteScalarAsync();

        count.ShouldBe(2);

        var reader = await command.ExecuteReaderAsync();

        var columns = DbReadHelper.GetColumns(reader);

        columns.Any().ShouldBeTrue();
    }

    [Fact]
    public async Task Exist()
    {
        (await ExistedDatabaseAsync("Data Source=:memory:")).ShouldBeTrue();

        (await ExistedDatabaseAsync("Data Source=./aaaaa.db")).ShouldBeFalse();
    }

    private const int SQLITE_CANTOPEN = 14;

    private static async Task<bool> ExistedDatabaseAsync(string connectionString)
    {
        _ = Check.NotNullOrWhiteSpace(connectionString);

        var stringBuilder = new SqliteConnectionStringBuilder(connectionString);

        // memory is always true
        if(stringBuilder.DataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase)
            || stringBuilder.Mode == SqliteOpenMode.Memory)
        {
            return true;
        }

        var readonlyStringBuilder = new SqliteConnectionStringBuilder(connectionString)
        {
            Mode = SqliteOpenMode.ReadOnly,
            Pooling = false
        };

        using var connection = new SqliteConnection(readonlyStringBuilder.ToString());

        try
        {
            await connection.OpenAsync();
        }
        catch(SqliteException ex) when(ex.SqliteErrorCode == SQLITE_CANTOPEN)
        {
            return false;
        }

        return true;
    }

    private static async Task EnsureCreateDatabase(string connectionString)
    {
        if(!await ExistedDatabaseAsync(connectionString))
        {
            using var connection = new SqliteConnection(connectionString);

            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = "PRAGMA journal_mode = 'wal';";

            _ = await command.ExecuteNonQueryAsync();

            command.Parameters.Clear();
        }
    }
}
