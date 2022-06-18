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

using System.Text;
using Shouldly;
using Xunit;

public class DisposeAction_Test
{
    [Fact]
    public void Dispose_StringBuilder()
    {
        var stringBuilder = new StringBuilder();

        using(var _ = new DisposeAction(() => stringBuilder.Clear()))
        {
            var word = nameof(Dispose_StringBuilder);

            stringBuilder.Append(word);

            stringBuilder.Length.ShouldBe(word.Length);
        }

        stringBuilder.Length.ShouldBe(0);
    }
}
