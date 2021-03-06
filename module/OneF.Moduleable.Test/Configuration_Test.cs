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

namespace OneF.Moduleable;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OneF.Moduleable.Fakes;
using Shouldly;
using Xunit;

public class Configuration_Test : TestBase
{
    protected sealed override void ConfigureConfiguration(IConfigurationBuilder builder)
    {
        _ = builder.AddJsonFile("appsettings.json");
    }

    [Fact]
    public void Get_Connection_Strings()
    {
        var optins = GetRequiredService<IOptions<TestOptions>>().Value;

        optins.ConnectionStrings.ContainsKey("Default").ShouldBeTrue();

        var connectionString = Configuration.GetConnectionString("Default");

        connectionString.ShouldNotBeNullOrWhiteSpace();
    }
}
