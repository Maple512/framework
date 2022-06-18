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

namespace System.Threading;

using Shouldly;
using Xunit;

/// <summary>
/// <para>提供延迟初始化</para>
/// <para>docs: https://docs.microsoft.com/zh-cn/dotnet/api/system.threading.lazyinitializer?view=net-6.0</para>
/// <para>code: https://github.com/dotnet/runtime/blob/91d29373771d85a6ce2099065d6d2e7b0c229e33/src/libraries/System.Private.CoreLib/src/System/Threading/LazyInitializer.cs</para>
/// <para>这些例程避免了需要分配一个专用的迟缓初始化实例，而是使用引用来确保目标已在访问时进行了初始化</para>
/// <para>在某些并不常用的计算属性上使用，确保延迟初始化</para>
/// </summary>
public class LazyInitializer_Test
{
    private static object _lock = new();

    [Fact]
    public void Ensure_initialized()
    {
        var a = new Model();

        a.Value.ShouldNotBeNullOrEmpty();
    }

    private class Model
    {
        private string _value;

        public string Name { get; set; }

        public string Value => LazyInitializer.EnsureInitialized(ref _value, ref _lock, static () => nameof(Model));
    }
}
