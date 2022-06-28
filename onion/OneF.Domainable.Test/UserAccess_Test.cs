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

using System.Linq;
using System.Threading.Tasks;
using Dapper;
using OneF.DataAccessable;
using OneF.Domainable.Fakes;
using Shouldly;
using Xunit;

public class UserAccess_Test : TestBase
{
    [Fact]
    public async Task NewAsync()
    {
        const string name = "maple512";

        var _dbContext = GetRequiredService<EfCoreDbContext>();

        _dbContext.Users.Count().ShouldBe(0);

        await _dbContext.Users.AddAsync(new User(1, name, name));

        var result = await _dbContext.SaveChangesAsync();

        result.ShouldBe(1);

        _dbContext.Users.ToList().FirstOrDefault(x => x.Name == name).ShouldNotBeNull();
    }

    [Fact]
    public async Task Dapper_access()
    {
        var connectionFactory = GetRequiredService<ISqlConnectionFatory>();

        using var connection = connectionFactory.CreateConnection();

        var user = new User(10, "Maple512", "Maple512");

        var sql = $"insert into user (\"{nameof(User.Id)}\",\"{nameof(User.Name)}\",\"{nameof(User.DisplayName)}\") values(@{nameof(User.Id)},@{nameof(User.Name)},@{nameof(User.DisplayName)})";

        var result = await connection.ExecuteAsync(
            sql,
            new { Id = (long)(UserId)user.Id, user.Name, user.DisplayName });

        result.ShouldBe(1);
    }
}
