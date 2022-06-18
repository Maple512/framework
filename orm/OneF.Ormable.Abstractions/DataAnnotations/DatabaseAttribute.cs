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

namespace OneF.Ormable.DataAnnotations;

using System;
using OneF.Ormable.Database;

/// <summary>
/// 为<see cref="IDatabase"/>指定配置
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DatabaseAttribute : Attribute
{
    /// <summary>
    /// 默认配置名: <c>Default</c>
    /// </summary>
    public const string Default = nameof(Default);

    /// <summary>
    /// 指定该数据库的一些属性
    /// </summary>
    /// <param name="configurationName">数据库配置名</param>
    public DatabaseAttribute(string? configurationName = null)
    {
        ConfigurationName = configurationName ?? Default;
    }

    /// <summary>
    /// 数据库配置名
    /// </summary>
    public string ConfigurationName { get; }
}
