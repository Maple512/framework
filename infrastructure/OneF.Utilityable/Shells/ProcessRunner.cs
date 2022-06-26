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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OneF.Interop;

// source: https://github.com/dotnet/tye/blob/bb49c161641b9f182cba61641149006811f60dc2/src/Microsoft.Tye.Core/ProcessUtil.cs

/// <summary>
/// 如果输出出现乱码，尝试加一段：<code>Console.OutputEncoding = Encoding.UTF8;</code>
/// </summary>
public static class ProcessRunner
{
    public static ProcessRunnerParameter Start(string commamd, string args)
        => new(commamd, args);

    public static ProcessRunnerParameter Start(string commamd, params string[] args)
        => new(commamd, args);

    public static async ValueTask<int> RunAsync(string command, params string[] args)
        => await RunAsync(new(command, args));

    /// <summary>
    /// 使用指定的参数运行一个进程
    /// </summary>
    /// <param name="paramter">参数</param>
    /// <param name="continueWithErrors">遇到错误是否继续</param>
    /// <param name="token">取消令牌</param>
    /// <returns></returns>
    public static async ValueTask<int> RunAsync(
        ProcessRunnerParameter paramter,
        bool continueWithErrors = true,
        CancellationToken token = default)
    {
        var arguments = paramter.GetArguments();

        using var process = new Process
        {
            StartInfo =
            {
                FileName = paramter.FileName,
                Arguments = arguments,
                WorkingDirectory = paramter.WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding =  Encoding.UTF8,
                StandardErrorEncoding =  Encoding.UTF8,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            },
            EnableRaisingEvents = true,
        };

        Debug.WriteLine($"Running: `{paramter.FileName} {arguments}`");

        if(paramter.Environments.Any())
        {
            foreach(var env in paramter.Environments)
            {
                process.StartInfo.Environment.Add(env);
            }
        }

        if(paramter.EnvironmentsToRemove.Any())
        {
            foreach(var env in paramter.EnvironmentsToRemove)
            {
                process.StartInfo.Environment.Remove(env);
            }
        }

        process.OutputDataReceived += (_, e) =>
        {
            paramter.OutputReceiver?.Invoke(true, e.Data);

            Debug.WriteLine($"OutputData: {e.Data}");
        };

        process.ErrorDataReceived += (_, e) =>
        {
            paramter.OutputReceiver?.Invoke(false, e.Data);

            Debug.WriteLine($"ErrorData: {e.Data}");
        };

        var processLifetimeTask = new TaskCompletionSource<int>();

        process.Exited += (_, e) =>
        {
            if(continueWithErrors && process.ExitCode != 0)
            {
                _ = processLifetimeTask.TrySetException(new InvalidOperationException($"Command: '{paramter.FileName} {arguments}', returned exit code: {process.ExitCode}"));
            }
            else
            {
                _ = processLifetimeTask.TrySetResult(process.ExitCode);
            }
        };

        _ = process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        var cancelledTcs = new TaskCompletionSource<object?>();

        await using var registration = token.Register(() => cancelledTcs.TrySetResult(null));

        var result = await Task.WhenAny(processLifetimeTask.Task, cancelledTcs.Task);

        if(result == cancelledTcs.Task)
        {
            Kill(process);

            if(!process.HasExited)
            {
                var cancel = new CancellationTokenSource();

                _ = await Task.WhenAny(processLifetimeTask.Task, Task.Delay(TimeSpan.FromSeconds(5), cancel.Token));

                cancel.Cancel();

                if(!process.HasExited)
                {
                    process.Kill();
                }
            }
        }

        return await processLifetimeTask.Task;
    }

    public static int Run(string command, params string[] args)
        => Run(new ProcessRunnerParameter(command, args));

    /// <summary>
    /// 使用指定的参数运行一个进程
    /// </summary>
    /// <param name="paramter">参数</param>
    /// <returns></returns>
    public static int Run(ProcessRunnerParameter paramter)
    {
        var arguments = paramter.GetArguments();

        using var process = new Process
        {
            StartInfo =
            {
                FileName = paramter.FileName,
                Arguments = arguments,
                WorkingDirectory = paramter.WorkingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding =  Encoding.UTF8,
                StandardErrorEncoding =  Encoding.UTF8,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            },
            EnableRaisingEvents = true,
        };

        Debug.WriteLine($"Running: `{paramter.FileName} {arguments}`");

        if(paramter.Environments.Any())
        {
            foreach(var env in paramter.Environments)
            {
                process.StartInfo.Environment.Add(env);
            }
        }

        if(paramter.EnvironmentsToRemove.Any())
        {
            foreach(var env in paramter.EnvironmentsToRemove)
            {
                _ = process.StartInfo.Environment.Remove(env);
            }
        }

        process.OutputDataReceived += (_, e) =>
        {
            paramter.OutputReceiver?.Invoke(true, e.Data);

            Debug.WriteLine($"OutputData: {e.Data}");
        };

        process.ErrorDataReceived += (_, e) =>
        {
            paramter.OutputReceiver?.Invoke(false, e.Data);

            Debug.WriteLine($"ErrorData: {e.Data}");
        };

        _ = process.Start();

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();

        Kill(process);

        if(!process.HasExited)
        {
            process.Kill();
        }

        return process.ExitCode;
    }

    public static void Kill(Process process)
    {
        if(!OSPlatformHelper.IsWindows)
        {
            _ = Interop.Libc.Kill(process.Id, sig: 2); // SIGINT
        }
        else
        {
            if(!process.CloseMainWindow())
            {
                process.Kill();
            }
        }
    }
}
