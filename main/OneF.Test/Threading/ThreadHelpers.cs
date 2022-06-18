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

namespace OneF.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

// code: https://github.com/dotnet/runtime/blob/542dc938a5e0f924fbfef435144e03c6bcf4dcc7/src/libraries/Common/tests/System/Threading/ThreadTestHelpers.cs
public static class ThreadHelpers
{
    public const int ExpectedTimeoutMilliseconds = 50;
    public const int UnexpectedTimeoutMilliseconds = 1000 * 60;

    /// <summary>
    /// A timeout (milliseconds) after which a wait on a remote operation should be considered a failure.
    /// </summary>
    /// <remarks>code: https://github.com/dotnet/arcade/blob/59775387deb609d7c62f9e713d133c34ba28ffcd/src/Microsoft.DotNet.RemoteExecutor/src/RemoteExecutor.cs#L22</remarks>
    public const int FailWaitTimeoutMilliseconds = 60 * 1000;

    // why use RemoteExecutor: https://github.com/dotnet/machinelearning/issues/3164

    // Wait longer for a thread to time out, so that an unexpected timeout in the thread is more
    // likely to expire first and provide a better stack trace for the failure
    public const int UnexpectedThreadTimeoutMilliseconds =
        UnexpectedTimeoutMilliseconds + FailWaitTimeoutMilliseconds;

    /// <summary>
    /// 创建守护线程
    /// </summary>
    /// <param name="waitForThread">等待线程执行完之后，</param>
    /// <param name="run">守护线程执行的任务</param>
    /// <returns></returns>
    public static Thread CreateGuardedThread(out Action waitForThread, Action run)
    {
        return CreateGuardedThread(out var _, out waitForThread, run);
    }

    /// <summary>
    /// 创建守护线程
    /// </summary>
    /// <param name="checkForThreadErrors">如果在执行 <paramref name="run"/> 时，有任何异常，都会从这里抛出</param>
    /// <param name="waitForThread">等待线程执行完之后，</param>
    /// <param name="run">守护线程执行的任务</param>
    /// <returns></returns>
    /// <exception cref="AggregateException"></exception>
    public static Thread CreateGuardedThread(out Action checkForThreadErrors, out Action waitForThread, Action run)
    {
        // 多线程共享变量，需要防止任何形式的更改指令或被缓存，这里通过Interlocked.MemoryBarrier来提供保护 Interlocked.MemoryBarrier 非阻塞同步
        Exception? backgroundEx = null;

        /*
         * 多线程共享变量
         * 编译器，CLR或CPU可能会更改指定的顺序来提高性能
         * 编译器，CLR或CPU可能会通过缓存来优化变量，这种情况下对其他线程不可见
         * 在C#中，以下操作都会生成MemoryBarrier
         * Lock语句（Monitor.Enter, Monitor.Exit）
         * 所有Interlocked类中的方法
         * 线程池的回调方法
         * Set或者Wait信号
         * 所有依赖于信号灯实现的方法，如starting或waiting一个Task
        */

        var t = new Thread(() =>
        {
            try
            {
                run();
            }
            catch(Exception ex)
            {
                backgroundEx = ex;

                Interlocked.MemoryBarrier();
            }
        });

        // cannot use ref or out parameters in lambda
        var localCheckForThreadErrors = checkForThreadErrors = () =>
        {
            Interlocked.MemoryBarrier();

            if(backgroundEx != null)
            {
                throw new AggregateException(backgroundEx);
            }
        };

        waitForThread = () =>
        {
            // thread join 阻塞其它线程，直到该线程终止或经过指定的时间
            t.Join(UnexpectedThreadTimeoutMilliseconds).ShouldBeTrue();

            localCheckForThreadErrors();
        };

        return t;
    }

    public static Thread CreateGuardedThread(out Action waitForThread, Action<object?> run)
    {
        return CreateGuardedThread(out var checkForThreadErrors, out waitForThread, run);
    }

    /// <summary>
    /// 创建守护线程
    /// </summary>
    /// <param name="checkForThreadErrors">如果在执行 <paramref name="run"/> 时，有任何异常，都会从这里抛出</param>
    /// <param name="waitForThread">等待线程执行完之后，</param>
    /// <param name="run">守护线程执行的任务</param>
    /// <returns></returns>
    /// <exception cref="AggregateException"></exception>
    public static Thread CreateGuardedThread(out Action checkForThreadErrors, out Action waitForThread, Action<object?> run)
    {
        Exception? backgroundEx = null;

        var t = new Thread(parameter =>
        {
            try
            {
                run(parameter);
            }
            catch(Exception ex)
            {
                backgroundEx = ex;

                Interlocked.MemoryBarrier();
            }
        });

        // cannot use ref or out parameters in lambda
        var localCheckForThreadErrors = checkForThreadErrors = () =>
        {
            Interlocked.MemoryBarrier();

            if(backgroundEx != null)
            {
                throw new AggregateException(backgroundEx);
            }
        };

        waitForThread = () =>
        {
            Assert.True(t.Join(UnexpectedThreadTimeoutMilliseconds));

            localCheckForThreadErrors();
        };

        return t;
    }

    /// <summary>
    /// 指定一个Action运行在后台线程中
    /// </summary>
    /// <param name="action"></param>
    public static void RunInBackgroundThread(Action action)
    {
        var t = CreateGuardedThread(out var waitForThread, action);

        t.IsBackground = true;

        t.Start();

        waitForThread();
    }

    /// <summary>
    /// 指定一个Func运行在后台线程中
    /// </summary>
    /// <param name="test"></param>
    public static void RunInBackgroundThread(Func<Task> test)
    {
        RunInBackgroundThread(() => test().Wait());
    }

    public static void WaitForCondition(Func<bool> condition)
    {
        WaitForConditionWithCustomDelay(condition, () => Thread.Sleep(1));
    }

    public static void WaitForConditionWithoutBlocking(Func<bool> condition)
    {
        WaitForConditionWithCustomDelay(condition, () => Thread.Yield());
    }

    public static void WaitForConditionWithoutRelinquishingTimeSlice(Func<bool> condition)
    {
        WaitForConditionWithCustomDelay(condition, () => Thread.SpinWait(1));
    }

    public static void WaitForConditionWithCustomDelay(Func<bool> condition, Action delay)
    {
        if(condition())
        {
            return;
        }

        var startTimeMs = Environment.TickCount;

        while(true)
        {
            delay();

            if(condition())
            {
                return;
            }

            (Environment.TickCount - startTimeMs).ShouldBeInRange(0, UnexpectedTimeoutMilliseconds);
        }
    }

    public static void CheckedWait(this WaitHandle wh, int millisecondsTimeout = UnexpectedTimeoutMilliseconds)
    {
        wh.WaitOne(millisecondsTimeout).ShouldBeTrue();
    }

    public static void CheckedWait(this ManualResetEventSlim e, int millisecondsTimeout = UnexpectedTimeoutMilliseconds)
    {
        e.Wait(millisecondsTimeout).ShouldBeTrue();
    }

    /// <summary>
    /// 等待任务是否能在指定的时间内内完成
    /// </summary>
    /// <param name="t"></param>
    /// <param name="millisecondsTimeout">任务超时时间（默认：60s）</param>
    public static void CheckedWait(this Task t, int millisecondsTimeout = UnexpectedTimeoutMilliseconds)
    {
        t.Wait(millisecondsTimeout).ShouldBeTrue();
    }
}
