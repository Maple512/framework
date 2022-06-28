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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Shouldly;
using Xunit;

public unsafe class Sizeof_Test
{
    [Fact]
    public void Cal_struct_size()
    {
        // -128 到 127
        sizeof(sbyte).ShouldBe(1);

        // 0 到 255
        sizeof(byte).ShouldBe(1);

        // 1 / 0
        sizeof(bool).ShouldBe(1);

        // 	-32,768 到 32,767
        sizeof(short).ShouldBe(2);

        // 	0 到 65,535
        sizeof(ushort).ShouldBe(2);

        sizeof(char).ShouldBe(2);

        // 	-2,147,483,648 到 2,147,483,647
        sizeof(int).ShouldBe(4);

        // 	0 到 4,294,967,295
        sizeof(uint).ShouldBe(4);

        // -9,223,372,036,854,775,808 到 9,223,372,036,854,775,807
        sizeof(long).ShouldBe(8);

        // 0 到 18,446,744,073,709,551,615
        sizeof(ulong).ShouldBe(8);

        sizeof(Guid).ShouldBe(16);

        sizeof(MultiStruct).ShouldBe(32);
        sizeof(MultiStruct1).ShouldBe(32);

        sizeof(AutoMultiStruct).ShouldBe(32);

        sizeof(ExplicitMultiStruct).ShouldBe(32);

        sizeof(DateTime).ShouldBe(8);

        sizeof(DateOnly).ShouldBe(4);

        sizeof(TimeOnly).ShouldBe(8);

        sizeof(DateTimeOffset).ShouldBe(16);

        Unsafe.SizeOf<Type>().ShouldBe(8);

        Unsafe.SizeOf<TypeInfo>().ShouldBe(8);
    }

    public struct MultiStruct
    {
        public bool Bool { get; set; }
        public char Char { get; set; }
        public int Int { get; set; }
        public long Long { get; set; }
        public Guid GuidStruct { get; set; }
    }

    public struct MultiStruct1
    {
        public bool Bool1;
        public char Char1;
        public int Int1;
        public long Long1;
        public Guid GuidStruct1;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ExplicitMultiStruct
    {
        [FieldOffset(0)]
        public bool Bool;

        [FieldOffset(1)]
        public char Char;

        [FieldOffset(3)]
        public int Int;

        [FieldOffset(7)]
        public long Long;

        [FieldOffset(15)]
        public Guid GuidStruct;
    }

    [StructLayout(LayoutKind.Auto)]
    public struct AutoMultiStruct
    {
        public bool Bool { get; set; }
        public char Char { get; set; }
        public int Int { get; set; }
        public long Long { get; set; }
        public Guid GuidStruct { get; set; }
    }
}
