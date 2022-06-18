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
using Microsoft.Extensions.DependencyInjection;
using OneF.Ormable.Database;
using OneF.Ormable.Entities;
using OneF.Ormable.Fakes;
using Shouldly;
using Xunit;

public class Repository_Test : TestBase
{
    private readonly IRepository<User, SqliteDbContext1> _users;

    public Repository_Test()
    {
        _users = GetRequiredService<IRepository<User, SqliteDbContext1>>();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddTransient<IConnectionStringProvider, MemoryConnectionProvider>();
        _ = services.AddTransient<IRepository<User, SqliteDbContext1>, Repository<User>>();
        _ = services.AddTransient<IDbConnectionFactory, SqliteConnectionFactory>();
    }

    [Fact]
    public async Task Connection_Test()
    {
        var hasTables = await _users.HasTableAsync(new User
        {
            Name = "123"
        });

        hasTables.ShouldBe(false);
    }
}
