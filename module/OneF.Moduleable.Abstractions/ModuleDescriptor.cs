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
using System.Linq.Expressions;
using System.Reflection;

public sealed class ModuleDescriptor : IModuleDescriptor
{
    private readonly List<IModuleDescriptor> _dependencies = new();

    public ModuleDescriptor(Type instance)
    {
        StartupType = Check.NotNull(instance);
        Assembly = instance.Assembly;

        if(instance.IsAssignableTo<IModule>() && instance.TryGetParameterlessConstructor(out var constructorInfo))
        {
            Module = Expression.Lambda<Func<IModule>>(Expression.New(constructorInfo)).Compile()();
        }
    }

    public Type StartupType { get; }

    public Assembly Assembly { get; }

    public IModule? Module { get; }

    public IReadOnlyList<IModuleDescriptor> Dependencies => _dependencies;

    /// <summary>
    /// 添加模块依赖
    /// </summary>
    /// <param name="descriptor"></param>
    public void AddDependency(IModuleDescriptor descriptor)
    {
        _dependencies.AddIfNotContains(descriptor);
    }

    public override string ToString()
    {
        return $"Module: {StartupType.GetShortDisplayName()}";
    }
}
