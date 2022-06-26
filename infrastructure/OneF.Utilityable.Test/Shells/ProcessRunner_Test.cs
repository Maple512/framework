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

namespace OneF.Shells;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

public class ProcessRunner_Test
{
    public ProcessRunner_Test()
    {
        if(Console.IsOutputRedirected)
        {
            Console.OutputEncoding = Encoding.UTF8;
        }

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    [Fact]
    public void Run_sync()
    {
        ProcessRunner.Start("dotnet", "--info")
            .Run().ShouldBe(0);
    }

    [Fact]
    public async Task Run_dotnet_info()
    {
        var content = new List<string?>();

        (await ProcessRunner.Start("dotnet", "--info")
            .WithOutput((_, msg) => content.Add(msg))
            .RunAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task Use_Cmd_Run_Command()
    {
        var content = new List<string?>();

        (await ProcessRunner.Start("dotnet", "--info")
            .WithOutput((_, msg) => content.Add(msg))
            .UseCmd()
            .RunAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task Run_nuget_search()
    {
        var content = new List<string?>();

        (await ProcessRunner.Start("dotnet", "tool search OneS.MediateS.Cli --detail --prerelease")
            .WithOutput((_, msg) => content.Add(msg))
            .RunAsync()).ShouldBe(0);
    }
}
