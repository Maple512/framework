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

using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Extras.DynamicProxy;
using OneF.IoCable.Abstractions;
using OneF.Proxyable;

namespace OneF.IoCable;

public static class OneFRegistrationBuilderExtensions
{
    public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> ConfigureConventions<TLimit, TActivatorData, TRegistrationStyle>(
    this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder)
        where TActivatorData : ReflectionActivatorData
    {
        var serviceType = registrationBuilder.RegistrationData.Services
                                             .OfType<IServiceWithType>()
                                             .FirstOrDefault()
                                             ?.ServiceType;

        if(serviceType == null)
        {
            return registrationBuilder;
        }

        var implementationType = registrationBuilder.ActivatorData.ImplementationType;
        if(implementationType == null)
        {
            return registrationBuilder;
        }

        // 属性注入
        if(implementationType.IsDefined<PerpertiesInjectAttribute>() || implementationType.Assembly.GetCustomAttribute<PerpertiesInjectAttribute>() != null)
        {
            registrationBuilder = registrationBuilder.PropertiesAutowired();
        }

        // 拦截器
        registrationBuilder = registrationBuilder.InvokeRegistrationActions(serviceType, implementationType);

        return registrationBuilder;
    }

    private static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InvokeRegistrationActions<TLimit, TActivatorData, TRegistrationStyle>(
    this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder,
    Type serviceType,
    Type implementationType)
        where TActivatorData : ReflectionActivatorData
    {
        var interceptors = serviceType.GetCustomAttributes<OneFInterceptorAttribute>()
                                      .Concat(implementationType.GetCustomAttributes<OneFInterceptorAttribute>())
                                      .SelectMany(x => x.Interceptors);

        if(interceptors.Any())
        {
            // 启用接口拦截
            if(serviceType.IsInterface)
            {
                registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
            }
            else
            {
                // 启用类拦截
                (registrationBuilder as IRegistrationBuilder<TLimit, ConcreteReflectionActivatorData, TRegistrationStyle>)?.EnableClassInterceptors();
            }

            foreach(var interceptor in interceptors)
            {
                registrationBuilder.InterceptedBy(
                    typeof(OneFAsyncDeterminationInterceptor<>).MakeGenericType(interceptor)
                );
            }
        }

        return registrationBuilder;
    }
}
