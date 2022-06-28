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

namespace OneF.Domainable.Fakes;

using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using OneF.DataAccessable;
using OneF.Moduleable.DependencyInjection;

public class SqlConnectionFatory : ISqlConnectionFatory, ITransientService
{
    private readonly SqlConnectionOptions _options;

    public SqlConnectionFatory(IOptionsSnapshot<SqlConnectionOptions> options)
    {
        _options = options.Value;
    }

    public DbConnection CreateConnection()
    {
        var connection = new SqliteConnection(Check.NotNullOrWhiteSpace(_options.ConnectionString));

        connection.Open();

        return connection;
    }
}
