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
using Microsoft.Extensions.DependencyInjection;

internal static class ModuleHelper
{
    private static Action<string>? _logger;

    public static IReadOnlyList<IModuleDescriptor> LoadModules(
    IServiceCollection services,
    Type startupModuleType,
    Action<string>? logger = null)
    {
        _ = Check.NotNull(services);
        _ = Check.NotNull(startupModuleType);

        _logger = logger;

        var modules = GetDescriptors(startupModuleType);

        modules = SortByDependency(modules, startupModuleType);

        return modules;
    }

    private static List<IModuleDescriptor> GetDescriptors(Type moduleType)
    {
        var modules = new List<IModuleDescriptor>();

        FillAllModules(modules, moduleType);

        FillAllDependencies(modules);

        return modules;
    }

    private static List<IModuleDescriptor> SortByDependency(
    IEnumerable<IModuleDescriptor> modules,
    Type startupModuleType)
    {
        var sortedModules = modules.SortByDependencies(m => m.Dependencies);

        sortedModules.MoveItem(m => m.StartupType == startupModuleType, modules.Count() - 1);

        return sortedModules;
    }

    /// <summary>
    /// 填充所有模块
    /// </summary>
    /// <param name="modules"></param>
    /// <param name="moduleType"></param>
    /// <returns></returns>
    private static void FillAllModules(ICollection<IModuleDescriptor> modules, Type moduleType)
    {
        _logger?.Invoke("Loading module starts.");

        foreach(var item in FindAllDependedTypes(moduleType))
        {
            modules.Add(new ModuleDescriptor(item));
        }

        _logger?.Invoke("Loading module ends.");
    }

    private static void FillAllDependencies(List<IModuleDescriptor> modules)
    {
        foreach(var module in modules)
        {
            FillDependencies(modules, module);
        }
    }

    /// <summary>
    /// 填充模块依赖
    /// </summary>
    /// <param name="allModules"></param>
    /// <param name="module"></param>
    private static void FillDependencies(
    IEnumerable<IModuleDescriptor> allModules,
    IModuleDescriptor module)
    {
        var moduleTypes = ModuleDependOnAttribute.GetAllDependedTypes(module.StartupType);

        foreach(var moduleType in moduleTypes)
        {
            var depended = allModules.FirstOrDefault(x => x.StartupType == moduleType);

            if(depended == null)
            {
                throw new ArgumentException(
                    $"Can not find a depended module {moduleType.AssemblyQualifiedName} for {module.StartupType.AssemblyQualifiedName}");
            }

            module.AddDependency(depended);
        }
    }

    private static List<Type> FindAllDependedTypes(Type moduleType)
    {
        var moduleTypes = new List<Type>();

        AddModuleWithDependenciesRecursively(moduleTypes, moduleType);

        return moduleTypes;
    }

    public static bool IsModule(Type moduleType)
    {
        return moduleType.IsClass
               && !moduleType.IsAbstract
               && !moduleType.IsGenericType;
    }

    private static void AddModuleWithDependenciesRecursively(
    ICollection<Type> moduleTypes,
    Type moduleType,
    int depth = 0)
    {
        if(moduleTypes.Contains(moduleType))
        {
            return;
        }

        moduleTypes.AddIfNotContains(moduleType);

        _logger?.Invoke(
            string.Format(
                "{0}- {1}", new string(' ', depth),
                moduleType.FullName));

        var dependTypes = ModuleDependOnAttribute.GetAllDependedTypes(moduleType);

        foreach(var item in dependTypes)
        {
            AddModuleWithDependenciesRecursively(moduleTypes, item, depth + 1);
        }
    }
}
