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

namespace OneF.Moduleable.DependencyInjection;
using System;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 服务描述特性
/// </summary>
/// <remarks>描述服务是否可以或如何注册到容器</remarks>
[AttributeUsage(AttributeTargets.Class)]
public class ServiceDescribeAttribute : Attribute
{
    private ServiceDescribeAttribute()
    { }

    public ServiceDescribeAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    /// <summary>
    /// 默认配置
    /// </summary>
    public static readonly ServiceDescribeAttribute Default = new();

    public static readonly ServiceDescribeAttribute Onlyself = new() { IncludeOnlySelf = true };

    /// <summary>
    /// 服务生命周期
    /// </summary>
    public ServiceLifetime? Lifetime { get; init; }

    /// <summary>
    /// 尝试注册，如果已有，则不注册
    /// </summary>
    public bool TryRegister { get; init; }

    /// <summary>
    /// 禁止该服务注册到容器
    /// </summary>
    public bool IsDisabled { get; init; }

    /// <summary>
    /// 替换第一个注册到容器的同类型的服务
    /// </summary>
    public bool ReplaceFirst { get; init; }

    /// <summary>
    /// 注册时，是否除了注册服务接口（如果有），还要注册包括服务实现类型自身
    /// </summary>
    public bool IncludeSelf { get; init; }

    /// <summary>
    /// 是否只注册服务自身，不包括继承的接口（如果有）
    /// </summary>
    public bool IncludeOnlySelf { get; init; }
}
