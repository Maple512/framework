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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

/// <summary>
/// 模块测试基类
/// </summary>
/// <remarks>初始化服务容器及相关配置（服务容器、配置、日志）</remarks>
/// <typeparam name="TStartup"></typeparam>
public abstract class OneFModuleTestBase<TStartup>
    where TStartup : class
{
    protected OneFModuleTestBase()
    {
        var services = new ServiceCollection();

        ConfigureServices(services);

        Configuration = services.ConfigureServices<TStartup>(ConfigureConfiguration);

        ServiceProvider = services.BuildServiceProviderFromFactory();

        _ = ServiceProvider.ConfigureAsync().AsTask().GetAwaiter().GetResult();
    }

    protected IServiceProvider ServiceProvider { get; }

    protected IConfiguration Configuration { get; }

    protected ILogger Logger => ServiceProvider.GetRequiredService<ILogger<TStartup>>();

    /// <summary>
    /// 服务容器
    /// </summary>
    /// <param name="services"></param>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        _ = services.AddLogging(builder =>
        {
            _ = builder.AddDebug();
        });
    }

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void ConfigureConfiguration(IConfigurationBuilder builder)
    {
    }

    protected IEnumerable<T> GetServices<T>()
    {
        return ServiceProvider.GetServices<T>();
    }

    protected T? GetService<T>()
    {
        return ServiceProvider.GetService<T>();
    }

    protected T GetRequiredService<T>()
        where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}
