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

namespace OneF.Mechineable;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Win32;

/// <summary>
/// 注册表访问工具类
/// </summary>
[StackTraceHidden]
[DebuggerStepThrough]
public static class RegistryHelper
{
    /// <summary>
    /// 获取一个注册表中的值，可能为空
    /// </summary>
    /// <param name="registryHive">注册表中的顶级节点</param>
    /// <param name="subKey">注册表的键</param>
    /// <param name="name">注册表项的名称</param>
    /// <returns></returns>
    public static bool TryGetValue(
        RegistryHive registryHive,
        string subKey,
        string name,
        [NotNullWhen(true)] out string? value)
    {
        return TryGetValue<string>(registryHive, subKey, name, out value);
    }

    /// <summary>
    /// 获取一个注册表中的值，可能为空
    /// </summary>
    /// <param name="registryHive">注册表中的顶级节点</param>
    /// <param name="subKey">注册表的键</param>
    /// <param name="name">注册表项的名称</param>
    /// <returns></returns>
    public static bool TryGetValue<T>(
        RegistryHive registryHive,
        string subKey,
        string name,
        [NotNullWhen(true)] out T? value)
    {
        Check.NotNullOrWhiteSpace(subKey);

        Check.NotNullOrWhiteSpace(name);

        var windowsNTKey = RegistryKey.OpenBaseKey(registryHive, RegistryView.Registry64).OpenSubKey(subKey);

        value = (T?)windowsNTKey?.GetValue(name);

        if(value is string str)
        {
            return !str.IsNullOrWhiteSpace();
        }

        return value is not null;
    }
}
