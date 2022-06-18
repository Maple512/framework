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
using System.Collections.Generic;
using System.Linq;

public static class ServiceCollectionCommonExtensions
{
    /// <summary>
    /// 从容器中获取指定类型的所有实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IEnumerable<T> GetInstances<T>(this IServiceCollection services)
    {
        return services.Where(x => x.ServiceType == typeof(T))
                       .Select(x => x.ImplementationInstance)
                       .Cast<T>();
    }

    /// <summary>
    /// 从容器中获取指定类型的单个实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static T? GetSingleInstanceOrNull<T>(this IServiceCollection services)
    {
        return (T?)services
                   .FirstOrDefault(d => d.ServiceType == typeof(T))
                   ?.ImplementationInstance;
    }

    /// <summary>
    /// 从容器中获取指定类型的单个实例，如果没有找到，则会发生异常
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static T GetSingleInstance<T>(this IServiceCollection services)
    {
        var service = services.GetSingleInstanceOrNull<T>();

        if(service == null)
        {
            throw new ArgumentException($"Could not find singleton service: {typeof(T).AssemblyQualifiedName}");
        }

        return service;
    }
}
