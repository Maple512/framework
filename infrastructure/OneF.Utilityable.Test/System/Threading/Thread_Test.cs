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

using System.Diagnostics;
using Shouldly;
using Xunit;

public class Thread_Test
{
    /// <summary>
    /// <see cref="Thread.Join()"/> 阻塞其它线程，立即运行当前线程，直到当前线程终止
    /// </summary>
    [Fact]
    public void Thread_join()
    {
        _ = ThreadPool.SetMinThreads(Environment.ProcessorCount * 500, Environment.ProcessorCount * 500);

        var index1 = 0;
        var index2 = 0;

        // 主线程
        Thread.CurrentThread.Name = $"{nameof(Thread_Test)}.main";

        // 副线程
        var thread1 = new Thread(new ThreadStart(func1))
        {
            Name = $"{nameof(Thread_Test)}.{nameof(func1)}"
        };

        var thread2 = new Thread(new ThreadStart(func2))
        {
            Name = $"{nameof(Thread_Test)}.{nameof(func2)}"
        };

        for(var i = 0; i < 20; i++)
        {
            index1 = i;
            index2 = i;
            if(i == 10)
            {
                thread1.Start();
                thread1.Join();

                thread2.Start();
            }
            else
            {
                Debug.WriteLine($"{Thread.CurrentThread.Name}: {i}", "main");
            }
        }

        void func1()
        {
            Debug.WriteLine($"*********Begin***********", "func1");

            for(var i = 0; i < 10; i++)
            {
                index1 = i;
                Debug.WriteLine($"{Thread.CurrentThread.Name}: {i}", "func1");
            }

            Debug.WriteLine($"*********Over***********", "func1");
        }

        void func2()
        {
            Debug.WriteLine($"*********Begin***********", "func2");

            for(var i = 0; i < 10; i++)
            {
                index2 = i;
                Debug.WriteLine($"{Thread.CurrentThread.Name}: {i}", "func2");
            }

            Debug.WriteLine($"*********Over***********", "func2");
        }

        index1.ShouldBe(19);

        // 本地测试：9，github action (Microsoft Windows Server 2019 10.0.17763)测试：19
        index2.ShouldBeOneOf(9, 19);

        Debug.WriteLine($"*********执行完毕*********", "main");
    }
}
