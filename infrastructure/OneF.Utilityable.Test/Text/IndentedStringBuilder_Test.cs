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

namespace OneF.Text;

using Shouldly;
using Xunit;

public class IndentedStringBuilder_Test
{
    [Fact]
    public void Append_At_Start_With_Indent()
    {
        var stringBuilder = new IndentedStringBuilder();

        using(stringBuilder.Indent())
        {
            _ = stringBuilder.Append("Foo");
        }

        stringBuilder.ToString().ShouldBe($"{new string(' ', 4)}Foo");
    }
}
