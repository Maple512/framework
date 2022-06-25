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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>
/// <para><see cref="ProcessRunner"/> 的参数</para>
/// <para><c>CMD</c> 命令需要处理一下：https://github.com/dotnet/sdk/blob/873d79d861cbd001488414b9875e53acbeaed890/src/Tests/Microsoft.NET.TestFramework/Commands/SdkCommandSpec.cs#L22</para>
/// </summary>
public sealed class ProcessRunnerParameter
{
    /*
     cmd 命令参数解析：https://www.cnblogs.com/mq0036/p/5244892.html
     */
    private static readonly string[] _cmdPrefix = new[] { "/s", "/c" };

    public ProcessRunnerParameter(string fileName, string arguments)
    {
        FileName = Check.NotNullOrWhiteSpace(fileName);

        if(UseCmd(fileName))
        {
            Arguments.AddRange(_cmdPrefix);
        }

        Arguments.AddRange(arguments.Split(' '));
    }

    public ProcessRunnerParameter(string fileName, params string[] arguments)
    {
        FileName = Check.NotNullOrWhiteSpace(fileName);

        if(UseCmd(fileName))
        {
            Arguments.AddRange(_cmdPrefix);
        }

        Arguments.AddRange(arguments);
    }

    public string FileName { get; }

    public List<string> Arguments { get; } = new();

    public string? WorkingDirectory { get; set; }

    public Dictionary<string, string?> Environments { get; } = new();

    public List<string> EnvironmentsToRemove { get; } = new();

    /// <summary>
    /// shell中的输出接收器，<see langword="false"/> 表示 <see cref="Process.StandardError"/> 中的内容
    /// </summary>
    public Action<bool, string?>? OutputReceiver { get; set; }

    public ProcessRunnerParameter WithWorkingDirectory(string workingDirectory)
    {
        WorkingDirectory = Check.NotNull(workingDirectory);

        return this;
    }

    public ProcessRunnerParameter AddEnvironment(string name, string value)
    {
        _ = Check.NotNullOrWhiteSpace(name);

        Environments[name] = value;

        return this;
    }

    public ProcessRunnerParameter AddEnvironmentToRemove(string name)
    {
        _ = Check.NotNullOrWhiteSpace(name);

        EnvironmentsToRemove.Add(name);

        return this;
    }

    public ProcessRunnerParameter WithOutput(Action<bool, string?> output)
    {
        OutputReceiver = Check.NotNull(output);

        return this;
    }

    public ProcessRunnerParameter AddArguments(params string[] arguments)
    {
        Arguments.AddRange(arguments);

        return this;
    }

    public int Run() => ProcessRunner.Run(this);

    public ValueTask<int> RunAsync() => ProcessRunner.RunAsync(this);

    private static bool UseCmd(string fileName)
    {
        if(fileName.EndsWith("cmd", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return fileName.EndsWith("cmd.exe", StringComparison.OrdinalIgnoreCase);
    }
}
