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

namespace OneF.Commands;

using System;
using System.Diagnostics;
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
    public async Task Run_dotnet_info()
    {
        var parameters = new ProcessRunnerParameter("dotnet", "--info")
        {
            OutputReceiver = static (_, msg) =>
            {
                Debug.WriteLine(msg);
            }
        };

        var result = await ProcessRunner.RunAsync(parameters);

        result.ShouldBe(0);
    }

    [Fact]
    public async Task Use_Cmd_Run_Command()
    {
        var parameters = new ProcessRunnerParameter("cmd", "dotnet --info")
        {
            OutputReceiver = static (_, msg) =>
            {
                Debug.WriteLine(msg);
            }
        };

        var result = await ProcessRunner.RunAsync(parameters);

        result.ShouldBe(0);
    }
}
