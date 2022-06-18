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

namespace System.Threading;

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OneF.Threading;
using Shouldly;
using Xunit;

/// <summary>
/// <para>为多线程共享变量提供原子操作</para>
/// <para>docs: https://docs.microsoft.com/zh-cn/dotnet/api/system.threading.interlocked?view=net-6.0</para>
/// </summary>
public class Interlocked_Test
{
    [Fact]
    public void Multi_threads_access_the_same_value()
    {
        _ = ThreadPool.SetMinThreads(Environment.ProcessorCount * 500, Environment.ProcessorCount * 500);

        var value = 512;

        Interlocked.Decrement(ref value).ShouldBe(511);

        var threads = new List<Task>(20000);

        var count = 0;

        var threadIds = new List<int>();

        for(var i = 0; i < 10000; i++)
        {
            threads.Add(Task.Run(() =>
            {
                _ = Interlocked.Increment(ref count);

                threadIds.AddIfNotContains(Environment.CurrentManagedThreadId);
            }));

            threads.Add(Task.Run(() =>
            {
                _ = Interlocked.Decrement(ref count);

                threadIds.AddIfNotContains(Environment.CurrentManagedThreadId);
            }));
        }

        Task.WaitAll(threads.ToArray());

        Assert.Equal(0, count);

        // should be 2, but github action 1
        threadIds.Count.ShouldBeGreaterThanOrEqualTo(1);

        Debug.Print("ThreadIds: {0}", threadIds.JoinAsString());
    }

    [Fact]
    public void InterlockedIncrement_Multithreaded_Int32()
    {
        const int ThreadCount = 10;
        const int IterationCount = 100;

        var value = 0;
        var threadStarted = new AutoResetEvent(false);
        var startTest = new ManualResetEvent(false);

        void threadStart()
        {
            _ = threadStarted.Set();
            startTest.CheckedWait();
            for(var i = 0; i < IterationCount; ++i)
            {
                _ = Interlocked.Increment(ref value);
            }
        }

        var waitsForThread = new Action[ThreadCount];

        for(var i = 0; i < ThreadCount; ++i)
        {
            var t = ThreadHelpers.CreateGuardedThread(out waitsForThread[i], threadStart);
            t.IsBackground = true;
            t.Start();
            threadStarted.CheckedWait();
        }

        _ = startTest.Set();

        foreach(var waitForThread in waitsForThread)
        {
            waitForThread();
        }

        Assert.Equal(ThreadCount * IterationCount, Interlocked.CompareExchange(ref value, 0, 0));
    }

    [Fact]
    public void InterlockedCompareExchange_Multithreaded_Double()
    {
        const int ThreadCount = 10;
        const int IterationCount = 100;
        const double Increment = ((long)1 << 32) + 1;

        var value = 0D;

        // 表示线程同步事件，当发出信号时，该事件在释放单个等待线程后自动重置
        var threadStarted = new AutoResetEvent(false);

        // 表示线程同步事件，当发出信号时，必须手动重置该事件
        var startTest = new ManualResetEvent(false);

        void threadStart()
        {
            _ = threadStarted.Set();

            startTest.CheckedWait();

            for(var i = 0; i < IterationCount; ++i)
            {
                var oldValue = value;

                while(true)
                {
                    var valueBeforeUpdate = Interlocked.CompareExchange(ref value, oldValue + Increment, oldValue);
                    if(valueBeforeUpdate == oldValue)
                    {
                        break;
                    }

                    oldValue = valueBeforeUpdate;
                }
            }
        }

        var waitsForThread = new Action[ThreadCount];
        for(var i = 0; i < ThreadCount; ++i)
        {
            var t = ThreadHelpers.CreateGuardedThread(out waitsForThread[i], threadStart);

            t.IsBackground = true;
            t.Start();

            threadStarted.CheckedWait();
        }

        _ = startTest.Set();

        foreach(var waitForThread in waitsForThread)
        {
            waitForThread();
        }

        Assert.Equal(ThreadCount * IterationCount * Increment, Interlocked.CompareExchange(ref value, 0, 0));
    }

    [Fact]
    public void InterlockedAddAndRead_Multithreaded_Int64()
    {
        const int ThreadCount = 10;
        const int IterationCount = 100;
        const long Increment = ((long)1 << 32) + 1;

        var value = 0L;
        var threadStarted = new AutoResetEvent(false);
        var startTest = new ManualResetEvent(false);
        var completedThreadCount = 0;
        void threadStart()
        {
            _ = threadStarted.Set();
            startTest.CheckedWait();
            for(var i = 0; i < IterationCount; ++i)
            {
                _ = Interlocked.Add(ref value, Increment);
            }

            _ = Interlocked.Increment(ref completedThreadCount);
        }

        var checksForThreadErrors = new Action[ThreadCount];
        var waitsForThread = new Action[ThreadCount];
        for(var i = 0; i < ThreadCount; ++i)
        {
            var t =
                ThreadHelpers.CreateGuardedThread(out checksForThreadErrors[i], out waitsForThread[i], threadStart);
            t.IsBackground = true;
            t.Start();
            threadStarted.CheckedWait();
        }

        _ = startTest.Set();
        ThreadHelpers.WaitForConditionWithCustomDelay(
            () => completedThreadCount >= ThreadCount,
            () =>
            {
                var valueSnapshot = Interlocked.Read(ref value);
                Assert.Equal((int)valueSnapshot, (int)(valueSnapshot >> 32));

                foreach(var checkForThreadErrors in checksForThreadErrors)
                {
                    checkForThreadErrors();
                }

                Thread.Sleep(1);
            });
        foreach(var waitForThread in waitsForThread)
        {
            waitForThread();
        }

        Assert.Equal(ThreadCount, completedThreadCount);
        Assert.Equal(ThreadCount * IterationCount * Increment, Interlocked.Read(ref value));
    }

    [Fact]
    public void MemoryBarrierProcessWide()
    {
        // Stress MemoryBarrierProcessWide correctness using a simple AsymmetricLock

        var asymmetricLock = new AsymmetricLock();
        var threads = new List<Task>();
        var count = 0;
        for(var i = 0; i < 1000; i++)
        {
            threads.Add(Task.Run(() =>
            {
                for(var j = 0; j < 1000; j++)
                {
                    var cookie = asymmetricLock.Enter();
                    count++;
                    cookie.Exit();
                }
            }));
        }

        Task.WaitAll(threads.ToArray());
        Assert.Equal(1000 * 1000, count);
    }

    // Taking this lock on the same thread repeatedly is very fast because it has no interlocked
    // operations. Switching the thread where the lock is taken is expensive because of allocation
    // and FlushProcessWriteBuffers.
    private class AsymmetricLock
    {
        public class LockCookie
        {
            internal LockCookie(int threadId)
            {
                ThreadId = threadId;
                Taken = false;
            }

            public void Exit()
            {
                Debug.Assert(ThreadId == Environment.CurrentManagedThreadId);
                Taken = false;
            }

            internal readonly int ThreadId;
            internal bool Taken;
        }

        private LockCookie _current = new(-1);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static T VolatileReadWithoutBarrier<T>(ref T location)
        {
            return location;
        }

        // Returning LockCookie to call Exit on is the fastest implementation because of it works
        // naturally with the RCU pattern. The traditional Enter/Exit lock interface would require
        // thread local storage or some other scheme to reclaim the cookie. Returning LockCookie to
        // call Exit on is the fastest implementation because of it works naturally with the RCU
        // pattern. The traditional Enter/Exit lock interface would require thread local storage or
        // some other scheme to reclaim the cookie
        public LockCookie Enter()
        {
            var currentThreadId = Environment.CurrentManagedThreadId;

            var entry = _current;

            if(entry.ThreadId == currentThreadId)
            {
                entry.Taken = true;

                // If other thread started stealing the ownership, we need to take slow path.
                //
                // Make sure that the compiler won't reorder the read with the above write by
                // wrapping the read in no-inline method. RyuJIT won't reorder them today, but more
                // advanced optimizers might. Regular Volatile.Read would be too big of a hammer
                // because of it will result into memory barrier on ARM that we do not need here.
                if(VolatileReadWithoutBarrier(ref _current) == entry)
                {
                    return entry;
                }

                entry.Taken = false;
            }

            return EnterSlow();
        }

        private LockCookie EnterSlow()
        {
            // Attempt to steal the ownership. Take a regular lock to ensure that only one thread is
            // trying to steal it at a time.
            lock(this)
            {
                // We are the new fast thread now!
                var oldEntry = _current;
                _current = new LockCookie(Environment.CurrentManagedThreadId);

                // After MemoryBarrierProcessWide, we can be sure that the Volatile.Read done by the
                // fast thread will see that it is not a fast thread anymore, and thus it will not
                // attempt to enter the lock.
                Interlocked.MemoryBarrierProcessWide();

                // Keep looping as long as the lock is taken by other thread
                var sw = new SpinWait();
                while(oldEntry.Taken)
                {
                    sw.SpinOnce();
                }

                // We have seen that the other thread released the lock by setting Taken to false.
                // However, on platforms with weak memory ordering (ex: ARM32, ARM64) observing that
                // does not guarantee that the writes executed by that thread prior to releasing the
                // lock are all committed to the shared memory. We could fix that by doing the
                // release via Volatile.Write, but we do not want to add expense to every release on
                // the fast path. Instead we will do another MemoryBarrierProcessWide here.

                // NOTE: not needed on x86/x64
                Interlocked.MemoryBarrierProcessWide();

                _current.Taken = true;
                return _current;
            }
        }
    }
}
