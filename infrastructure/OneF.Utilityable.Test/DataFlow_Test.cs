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

namespace OneF;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Xunit;

public class DataFlow_Test
{
    // 素数：大于1的自然数，除了1和本身外，不会被其他自然数整除

    [Theory(DisplayName = "使用共享内存的方式计算素数")]
    [InlineData(10_000)]
    [InlineData(100_000)]
    public void Calculate_Prime_Number_Using_Shared_Memory(int maxNumber)
    {
        var sum = 0;

        _ = Parallel.For(
            1, maxNumber + 1, (x, state) =>
            {
                var flag = true;
                if(x == 1)
                {
                    flag = false;
                }

                for(var i = 2; i < x / 2; i++)
                {
                    if(x % i == 0)
                    {
                        flag = false;
                    }
                }

                if(flag)
                {
                    // sum++，共享sum
                    _ = Interlocked.Increment(ref sum);
                }
            });

        Debug.Print($"1 - {maxNumber}内素数的个数是{sum}");

        Assert.True(sum > 0);
    }

    [Theory(DisplayName = "使用Actor的方式计算素数")]
    [InlineData(10_000)]
    [InlineData(100_000)]
    public async void Calculate_Prime_Numver_Using_Actor(int maxNumber)
    {
        var linkOptions = new DataflowLinkOptions
        {
            PropagateCompletion = true,
        };

        var transfromBlock = new TransformBlock<int, bool>(
            x =>
            {
                var flag = true;

                if(x == 1)
                {
                    flag = false;
                }

                for(var i = 2; i <= x / 2; i++)
                {
                    if(x % i == 0)
                    {
                        flag = false;
                    }
                }

                return flag;
            }, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 50,
                EnsureOrdered = false,
            });

        var sum = 0;
        var actionBlock = new ActionBlock<bool>(
            x =>
            {
                if(x)
                {
                    sum++;
                }
            }, new ExecutionDataflowBlockOptions
            {
                EnsureOrdered = false,
            });

        _ = transfromBlock.LinkTo(actionBlock, linkOptions);

        // 准备从Pipeline头部开始投递
        try
        {
            var list = new List<int>();

            for(var i = 0; i < maxNumber; i++)
            {
                var flag = await transfromBlock.SendAsync(i);
                if(!flag)
                {
                    list.Add(i);
                }
            }

            if(list.Count > 0)
            {
                Debug.Print($"md, num post failure, num: {list.Count}, post again;");

                // 再投一次
                foreach(var item in list)
                {
                    _ = transfromBlock.Post(item);
                }
            }

            // 通知头部，不再投递了，会将信息传递到下游
            transfromBlock.Complete();

            // 等待尾部执行完
            actionBlock.Completion.Wait();

            Debug.Print($"1 - {maxNumber} prime number include {sum}");
        }
        catch(Exception ex)
        {
            Debug.Print($"1 - {maxNumber} cause exception.", ex);
        }

        Assert.True(sum > 0);
    }
}
