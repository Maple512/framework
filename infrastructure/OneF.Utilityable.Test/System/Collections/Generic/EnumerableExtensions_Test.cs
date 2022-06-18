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

namespace System.Collections.Generic;

using Shouldly;
using Xunit;

public class EnumerableExtensions_Test
{
    [Fact]
    public void Contain_any()
    {
        var source = new[] { 1, 2, 3, 4, 5 };

        source.ContainAny().ShouldBeFalse();
        source.ContainAny(1).ShouldBeTrue();
        source.ContainAny(1, 9).ShouldBeTrue();

        source.ContainAll().ShouldBeFalse();
        source.ContainAll(1, 2).ShouldBeTrue();
        source.ContainAll(1, 2, 8).ShouldBeFalse();
    }
}
