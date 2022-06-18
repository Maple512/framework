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

namespace Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OneF;
using OneF.Moduleable;

public static class ServiceCollectionModuleContextExtensions
{
    public static IConfiguration ConfigureServices<TStartup>(
        this IServiceCollection services,
        Action<IConfigurationBuilder>? configurationBuilderAction = null,
        Action<string>? logger = null)
    {
        return ModuleFactory.ConfigureServices<TStartup>(
            services,
            configurationBuilderAction,
            logger);
    }

    public static void ConfigureServices<TStartup>(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<string>? logger = null)
    {
        _ = Check.NotNull(configuration);

        ModuleFactory.ConfigureServices(
           typeof(TStartup),
           services,
           configuration,
           logger);
    }

    public static void ConfigureServices(
        this IServiceCollection services,
        Type startupType,
        IConfiguration configuration,
        Action<string>? logger = null)
    {
        _ = Check.NotNull(startupType);
        _ = Check.NotNull(configuration);

        ModuleFactory.ConfigureServices(
           startupType,
           services,
           configuration,
           logger);
    }

    public static async ValueTask<IServiceProvider> ConfigureAsync(this IServiceProvider serviceProvider)
    {
        return await ModuleFactory.ConfigureAsync(serviceProvider);
    }
}
