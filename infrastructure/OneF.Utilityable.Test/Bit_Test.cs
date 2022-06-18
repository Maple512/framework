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

using System.Collections;
using Shouldly;
using Xunit;

public class Bit_Test
{
    [Fact]
    public void Bit_conversion()
    {
        // 1int = 4byte = 32bit byte 0 ~ 255 = bit 0000 0000 ~ 1111 1111

        // 1bool -> 1bit
        new BitArray(new bool[] { true }).Length.ShouldBe(1);

        // 1byte -> 8bit
        new BitArray(new byte[] { 1 }).Length.ShouldBe(8);
        new BitArray(new byte[] { 255 }).Length.ShouldBe(8);

        // 1int -> 32bit
        new BitArray(new int[] { 1 }).Length.ShouldBe(32);
        new BitArray(new int[] { int.MaxValue }).Length.ShouldBe(32);
    }
}
