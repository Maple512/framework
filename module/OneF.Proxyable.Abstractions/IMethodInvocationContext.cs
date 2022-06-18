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
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace OneF.Proxyable;

/// <summary>
/// 方法调用上下文
/// </summary>
public interface IMethodInvocationContext
{
    /// <summary>
    /// 方法参数
    /// </summary>
    object[] Arguments { get; }

    /// <summary>
    /// 参数字典
    /// </summary>
    IReadOnlyDictionary<string, object> ArgumentDictionay { get; }

    /// <summary>
    /// 泛型参数
    /// </summary>
    Type[] GeneicArguments { get; }

    /// <summary>
    /// 绑定对象
    /// </summary>
    object TargetObject { get; }

    /// <summary>
    /// 方法
    /// </summary>
    MethodInfo Method { get; }

    /// <summary>
    /// 返回值
    /// </summary>
    object? ReturnValue { get; }

    /// <summary>
    /// 进入方法
    /// </summary>
    /// <returns></returns>
    Task ProceedAsync();
}
