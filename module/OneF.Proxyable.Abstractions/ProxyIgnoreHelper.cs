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

namespace OneF.Proxyable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// 代理忽略帮助器
/// </summary>
/// <remarks>
/// 并不是所有类都适合使用动态代理，参考：https://github.com/castleproject/Core/issues/486
/// https://github.com/abpframework/abp/issues/3180， 这里为不适合动态代理的类提供一个可忽略的选择。
/// </remarks>
public static class ProxyIgnoreHelper
{
    private static readonly HashSet<Type> _ignoreTypes = new();

    public static void Add<T>()
    {
        lock(_ignoreTypes)
        {
            _ignoreTypes.AddIfNotContains(typeof(T));
        }
    }

    /// <summary>
    /// 忽略的代理类中，是否包含指定的类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="isIncludeDerived">是否包括派生</param>
    /// <returns></returns>
    public static bool Contains<T>(bool isIncludeDerived = true)
    {
        lock(_ignoreTypes)
        {
            return isIncludeDerived
                ? _ignoreTypes.Any(x => x.IsAssignableFrom<T>())
                : _ignoreTypes.Any(x => x == typeof(T));
        }
    }
}
