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

namespace OneF.Domainable;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneF.DataAccessable;
using OneF.Domainable.Fakes;
using OneF.Moduleable;

public class TestBase : OneFModuleTestBase<TestBase>
{
    protected override void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Configuration.GetConnectionString("Default");

        services.AddDbContext<EfCoreDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.Configure<SqlConnectionOptions>(options =>
        {
            options.ConnectionString = connectionString;
        });
    }

    protected override void ConfigureConfiguration(IConfigurationBuilder builder)
    {
        builder.AddJsonFile("appsettings.json");
    }

    protected override void Initialize()
    {
        var _dbContext = GetRequiredService<EfCoreDbContext>();

        _dbContext.Database.EnsureDeleted();

        _dbContext.Database.EnsureCreated();
    }
}
