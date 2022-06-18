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

namespace OneF.Threadable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneF.Moduleable.DependencyInjection;
using Shouldly;
using Xunit;

public class CancellationToken_Test : ThreadTestBase
{
    private readonly ICancellationTokenProvider _cancellationTokenProvider;

    public CancellationToken_Test()
    {
        _cancellationTokenProvider = GetRequiredService<ICancellationTokenProvider>();
    }

    [Fact(DisplayName = "延迟取消任务")]
    public async Task Delay_Cancel()
    {
        var cancellationToken = _cancellationTokenProvider.Token;

        _ = cancellationToken.Register(
            () =>
            {
                Logger.LogInformation("注册一个取消事件");
            });

        var watch = StopwatchValue.StartNew();

        await Task.Run(
            async () =>
            {
                Logger.LogInformation("************************");

                while(!cancellationToken.IsCancellationRequested)
                {
                    Logger.LogInformation("任务在执行：{Secends}s", watch.GetElapsedTime().TotalSeconds);

                    await Task.Delay(1_000);
                }
            }, cancellationToken);

        watch.GetElapsedTime().Seconds.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact(DisplayName = "定时取消")]
    public async Task Timed_Cancellation()
    {
        var cts = new CancellationTokenSource();

        cts.CancelAfter(1_000 * 3);

        _ = await Should.ThrowAsync<TaskCanceledException>(
            async () =>
            {
                await Task.Run(
                    async () =>
                    {
                        while(true)
                        {
                            Logger.LogInformation("CancellationToken: {IsCancellationRequested}", cts.IsCancellationRequested);

                            await Task.Delay(1_000, cts.Token);
                        }
                    });
            });
    }
}

[ServiceDescribe(ServiceLifetime.Singleton, ReplaceFirst = true)]
public class DelayCancellationTokenProvider : CancellationTokenProviderBase
{
    protected override CancellationTokenSource? GetCancellationTokenSource()
    {
        return new CancellationTokenSource(2_000);
    }
}
