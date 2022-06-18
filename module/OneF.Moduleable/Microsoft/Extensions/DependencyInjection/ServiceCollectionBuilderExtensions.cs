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
using System.Linq;
using System.Reflection;
using OneF;

public static class ServiceCollectionBuilderExtensions
{
    public static IServiceProvider BuildServiceProviderFromFactory(this IServiceCollection services)
    {
        _ = Check.NotNull(services);

        foreach(var service in services)
        {
            var factoryInterface = service.ImplementationInstance?.GetType()
                .GetInterfaces()
                .FirstOrDefault(
                i => i.GetTypeInfo().IsGenericType
                && i.GetGenericTypeDefinition() == typeof(IServiceProviderFactory<>));

            if(factoryInterface == null)
            {
                continue;
            }

            var containerBuilderType = factoryInterface.GenericTypeArguments[0];

            return (IServiceProvider)typeof(ServiceCollectionBuilderExtensions)
                .GetMethods()
                .Single(m => m.Name == nameof(BuildServiceProviderWithContaner))
                .MakeGenericMethod(containerBuilderType)
                .Invoke(null, new object[]
                {
                    services, null!,
                })!;
        }

        return services.BuildServiceProvider();
    }

    public static IServiceProvider BuildServiceProviderWithContaner<TContainerBuilder>(
     this IServiceCollection services,
     Action<TContainerBuilder>? builderAction = null)
        where TContainerBuilder : notnull
    {
        _ = Check.NotNull(services);

        var serviceProviderFactory = services.GetSingleInstance<IServiceProviderFactory<TContainerBuilder>>();

        var builder = serviceProviderFactory.CreateBuilder(services);

        builderAction?.Invoke(builder);

        return serviceProviderFactory.CreateServiceProvider(builder);
    }
}
