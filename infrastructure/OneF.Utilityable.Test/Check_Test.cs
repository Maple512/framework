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

namespace OneF;

using System;
using Shouldly;
using Xunit;

public class Check_Test
{
    [Fact]
    public void Not_Null_Or_Empty()
    {
        string? str = null;

        try
        {
            _ = Check.NotNullOrWhiteSpace(str);
        }
        catch(ArgumentNullException ex)
        {
            nameof(str).ShouldBeEquivalentTo(ex.ParamName);
        }
    }

    [Fact]
    public void Debug_Assert()
    {
#if DEBUG
        Should.Throw<ArgumentException>(() =>
        {
            Check.DebugAssert(1 != 1, "1 should be equal to 1");
        });
#else
        Should.NotThrow(() =>
        {
            Check.DebugAssert(1 != 1, "1 should be equal to 1");
        });
#endif
    }
}
