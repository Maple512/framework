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

using System.Diagnostics;
using Microsoft.Win32;
using Shouldly;
using Xunit;

public class RegistryHelper_Test
{
    [Fact]
    public void Get_value()
    {
        RegistryHelper.TryGetValue(
            RegistryHive.CurrentConfig,
            "2",
            "2",
            out var _)
            .ShouldBeFalse();
    }

    /// <summary>
    /// 获取产品ID
    /// </summary>
    [Fact]
    public void Get_productid()
    {
        RegistryHelper.TryGetValue(
            RegistryHive.LocalMachine,
            "Software\\Microsoft\\Windows NT\\CurrentVersion",
            "ProductId",
            out var productId).ShouldBeTrue();

        Debug.WriteLine(productId);

        productId.ShouldNotBeNull();
    }
}
