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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ConfigureServiceContextExtensions
{
    #region Configure

    /// <inheritdoc cref="OptionsServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, Action{TOptions})"/>
    public static ConfigureServiceContext Configure<TOptions>(this ConfigureServiceContext context, Action<TOptions> configureOptions)
        where TOptions : class
    {
        _ = context.Services.Configure(configureOptions);

        return context;
    }

    /// <inheritdoc cref="OptionsServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, string, Action{TOptions})"/>
    public static ConfigureServiceContext Configure<TOptions>(this ConfigureServiceContext context, string name, Action<TOptions> configureOptions)
        where TOptions : class
    {
        _ = context.Services.Configure(name, configureOptions);

        return context;
    }

    /// <inheritdoc cref="OptionsServiceCollectionExtensions.ConfigureAll{TOptions}(IServiceCollection, Action{TOptions})"/>
    public static ConfigureServiceContext ConfigureAll<TOptions>(this ConfigureServiceContext context, Action<TOptions> configureOptions)
        where TOptions : class
    {
        _ = context.Services.ConfigureAll(configureOptions);

        return context;
    }

    /// <inheritdoc cref="OptionsConfigurationServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, IConfiguration)"/>
    public static ConfigureServiceContext Configure<TOptions>(this ConfigureServiceContext context, IConfiguration configuration)
        where TOptions : class
    {
        _ = context.Services.Configure<TOptions>(configuration);

        return context;
    }

    /// <inheritdoc cref="OptionsConfigurationServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, IConfiguration, Action{BinderOptions})"/>
    public static ConfigureServiceContext Configure<TOptions>(this ConfigureServiceContext context, IConfiguration configuration, Action<BinderOptions> configureBinder)
        where TOptions : class
    {
        _ = context.Services.Configure<TOptions>(configuration, configureBinder);

        return context;
    }

    /// <inheritdoc cref="OptionsConfigurationServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, string, IConfiguration)"/>
    public static ConfigureServiceContext Configure<TOptions>(this ConfigureServiceContext context, string name, IConfiguration configuration)
        where TOptions : class
    {
        _ = context.Services.Configure<TOptions>(name, configuration);

        return context;
    }

    /// <inheritdoc cref="OptionsConfigurationServiceCollectionExtensions.Configure{TOptions}(IServiceCollection, string, IConfiguration, Action{BinderOptions})"/>
    public static ConfigureServiceContext Configure<TOptions>(this ConfigureServiceContext context, string name, IConfiguration configuration, Action<BinderOptions> configureBinder)
        where TOptions : class
    {
        _ = context.Services.Configure<TOptions>(name, configuration, configureBinder);

        return context;
    }

    #endregion Configure

    #region PostConfigure

    /// <inheritdoc cref="OptionsServiceCollectionExtensions.PostConfigure{TOptions}(IServiceCollection, Action{TOptions})"/>
    public static ConfigureServiceContext PostConfigure<TOptions>(this ConfigureServiceContext context, Action<TOptions> configureOptions)
        where TOptions : class
    {
        _ = context.Services.PostConfigure(configureOptions);

        return context;
    }

    /// <inheritdoc cref="OptionsServiceCollectionExtensions.PostConfigure{TOptions}(IServiceCollection, string, Action{TOptions})"/>
    public static ConfigureServiceContext PostConfigure<TOptions>(this ConfigureServiceContext context, string name, Action<TOptions> configureOptions)
        where TOptions : class
    {
        _ = context.Services.PostConfigure(name, configureOptions);

        return context;
    }

    /// <inheritdoc cref="OptionsServiceCollectionExtensions.PostConfigureAll{TOptions}(IServiceCollection, Action{TOptions})"/>
    public static ConfigureServiceContext PostConfigureAll<TOptions>(this ConfigureServiceContext context, Action<TOptions> configureOptions)
        where TOptions : class
    {
        _ = context.Services.PostConfigureAll(configureOptions);

        return context;
    }

    #endregion PostConfigure
}
