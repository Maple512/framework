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
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneF.Moduleable.DependencyInjection;

public static class ModuleFactory
{
    public static IConfiguration ConfigureServices<TStartup>(
        IServiceCollection? services = null,
        Action<IConfigurationBuilder>? configurationBuilderAction = null,
        Action<string>? logger = null)
    {
        return ConfigureServices(
            typeof(TStartup),
            services,
            configurationBuilderAction,
            logger);
    }

    public static IConfiguration ConfigureServices(
        Type startupType,
        IServiceCollection? services = null,
        Action<IConfigurationBuilder>? configurationBuilderAction = null,
        Action<string>? logger = null)
    {
        services ??= new ServiceCollection();

        var configurationBuilder = new ConfigurationBuilder();

        if(configurationBuilderAction == null)
        {
            configurationBuilderAction = ActionNull<IConfigurationBuilder>.Instance;
        }

        configurationBuilderAction.Invoke(configurationBuilder);

        var configuration = configurationBuilder.Build();

        ConfigureServices(
           startupType,
           services,
           configuration,
           logger);

        return configuration;
    }

    public static void ConfigureServices(
        Type startupType,
        IServiceCollection services,
        IConfiguration configuration,
        Action<string>? logger = null)
    {
        _ = Check.NotNull(startupType);
        _ = Check.NotNull(services);
        _ = Check.NotNull(configuration);

        _ = services.AddOptions();

        var modules = ModuleHelper.LoadModules(services, startupType, logger);

        var moduleContext = new ModuleContext(modules);

        _ = services.AddSingleton<IModuleContext>(moduleContext);

        ConfigureServices(modules, services, configuration);
    }

    public static async Task<IServiceProvider> ConfigureAsync(IServiceProvider serviceProvider)
    {
        var moduleContext = serviceProvider.GetRequiredService<IModuleContext>();

        await ConfigureAsync(moduleContext.Modules, serviceProvider);

        return serviceProvider;
    }

    private static void ConfigureServices(
        IReadOnlyList<IModuleDescriptor> modules,
        IServiceCollection services,
        IConfiguration configuration)
    {
        // Assembly scan
        foreach(var module in modules)
        {
            var context = new AssemblyScanContext(services, module.Assembly);

            if(module.Module is IAssemblyScanner scanner)
            {
                if(scanner.KeepDefault)
                {
                    // default
                    AssemblyScanHelper.RegisterAssembly(context);
                }

                scanner.Scan(context);
            }
            else
            {
                // default
                AssemblyScanHelper.RegisterAssembly(context);
            }
        }

        // Configure services
        var configureServiceContext = new ConfigureServiceContext(services, configuration);

        foreach(var module in modules)
        {
            if(module.Module is IConfigureServices configure)
            {
                configure.ConfigureServices(configureServiceContext);
            }
        }

        foreach(var module in modules)
        {
            if(module.Module is IPostConfigureServices postConfigure)
            {
                postConfigure.PostConfigureServices(configureServiceContext);
            }
        }
    }

    private static async Task ConfigureAsync(IReadOnlyList<IModuleDescriptor> modules, IServiceProvider serviceProvider)
    {
        var context = new ConfigureContext(serviceProvider);

        foreach(var module in modules)
        {
            if(module.Module is IConfigure configure)
            {
                await configure.ConfigureAsync(context);
            }
        }
    }
}
