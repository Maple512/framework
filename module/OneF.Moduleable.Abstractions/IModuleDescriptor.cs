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
using System.Reflection;

/// <summary>
/// 模块自述器
/// </summary>
public interface IModuleDescriptor
{
    /// <summary>
    /// 模块类型
    /// </summary>
    Type StartupType { get; }

    /// <summary>
    /// 模块所在的程序集
    /// </summary>
    Assembly Assembly { get; }

    /// <summary>
    /// 模块的实例
    /// </summary>
    IModule? Module { get; }

    /// <summary>
    /// 模块的依赖项
    /// </summary>
    IReadOnlyList<IModuleDescriptor> Dependencies { get; }

    void AddDependency(IModuleDescriptor descriptor);
}
