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
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using OneF.Ormable.Infrastructure;

public class SqliteDatabaseConnection : DatabaseConnection, ISqliteDatabaseConnection
{
    private readonly bool _loadSpatilaite;
    private readonly int? _commandTimeout;

    public SqliteDatabaseConnection(IConnectionOptions connectionInfo)
        : base(connectionInfo)
    {
        if(connectionInfo is SqliteConnectionOptions sqlite)
        {
            _loadSpatilaite = sqlite.LoadSpatialite;
        }

        _commandTimeout = connectionInfo.CommandTimeout;

        if(connectionInfo.DbConnection is not null)
        {
            Initialize(connectionInfo.DbConnection);
        }
    }

    public ISqliteDatabaseConnection CreateReadOnlyConnection()
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder(GetValidatedConnectionString())
        {
            Mode = SqliteOpenMode.ReadOnly,
            Pooling = false
        };

        return new SqliteDatabaseConnection(new SqliteConnectionOptions(connectionStringBuilder.ToString(), null, CommandTimeout));
    }

    protected override DbConnection CreateDbConnection()
    {
        var connection = new SqliteConnection(GetValidatedConnectionString());

        Initialize(connection);

        return connection;
    }

    private void Initialize(DbConnection connection)
    {
        if(_loadSpatilaite)
        {
            SpatialiteLoader.Load(connection);
        }

        if(connection is SqliteConnection sqliteConnection)
        {
            if(_commandTimeout.HasValue)
            {
                sqliteConnection.DefaultTimeout = _commandTimeout.Value;
            }

            sqliteConnection.CreateFunction<string, string, bool?>(
                "regexp",
                (pattern, input) =>
                {
                    if(input == null
                        || pattern == null)
                    {
                        return null;
                    }

                    return Regex.IsMatch(input, pattern);
                },
                isDeterministic: true);

            sqliteConnection.CreateFunction<object, object, object?>(
                "ef_mod",
                (dividend, divisor) =>
                {
                    if(dividend == null
                        || divisor == null)
                    {
                        return null;
                    }

                    if(dividend is string s)
                    {
                        return decimal.Parse(s, CultureInfo.InvariantCulture)
                            % Convert.ToDecimal(divisor, CultureInfo.InvariantCulture);
                    }

                    return Convert.ToDouble(dividend, CultureInfo.InvariantCulture)
                        % Convert.ToDouble(divisor, CultureInfo.InvariantCulture);
                },
                isDeterministic: true);

            sqliteConnection.CreateFunction(
                name: "ef_add",
                (decimal? left, decimal? right) => left + right,
                isDeterministic: true);

            sqliteConnection.CreateFunction(
                name: "ef_divide",
                (decimal? dividend, decimal? divisor) => dividend / divisor,
                isDeterministic: true);

            sqliteConnection.CreateFunction(
                name: "ef_compare",
                (decimal? left, decimal? right) => left.HasValue && right.HasValue
                    ? decimal.Compare(left.Value, right.Value)
                    : default(int?),
                isDeterministic: true);

            sqliteConnection.CreateFunction(
                name: "ef_multiply",
                (decimal? left, decimal? right) => left * right,
                isDeterministic: true);

            sqliteConnection.CreateFunction(
                name: "ef_negate",
                (decimal? m) => -m,
                isDeterministic: true);
        }

        // TODO: log to no sqlite connection
    }
}
