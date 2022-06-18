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
using System.Linq;
using System.Reflection;

/// <summary>
/// 指定该模块依赖的其他模块
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
public class ModuleDependOnAttribute : Attribute
{
    public ModuleDependOnAttribute(params Type[] denpendTypes)
    {
        DenpendedTypes = denpendTypes;
    }

    /// <summary>
    /// 依赖项
    /// </summary>
    public Type[] DenpendedTypes { get; }

    public static IEnumerable<Type> GetAllDependedTypes(Type type)
    {
        return GetAllDependedTypes(type, type.Assembly);
    }

    public static IEnumerable<Type> GetAllDependedTypes(Type type, Assembly assembly)
    {
        var attributes = type.GetCustomAttributes<ModuleDependOnAttribute>();

        if(attributes.IsNullOrEmpty())
        {
            attributes = assembly.GetCustomAttributes<ModuleDependOnAttribute>();
        }

        return attributes
               .SelectMany(x => x.DenpendedTypes)
               .Distinct();
    }
}
