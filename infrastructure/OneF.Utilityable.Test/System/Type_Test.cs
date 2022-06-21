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

namespace System;

using System.Collections.Generic;
using System.Reflection;
using Shouldly;
using Xunit;

public class Type_Test
{
    [Fact]
    public void Is_Anonymous_Type()
    {
        var anonymous = new { };

        var type = anonymous.GetType();

        type.IsAnonymousType().ShouldBeTrue();

        typeof(Type_Test).IsAnonymousType().ShouldBeFalse();
    }

    [Fact]
    public void Parameter_Less_Contructor()
    {
        _ = typeof(ParameterLessModel)
            .GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, Type.EmptyTypes)
            .ShouldNotBeNull();
    }

    [Fact]
    public void Static_Attribute()
    {
        var attrs = typeof(StaticClass).Attributes;

        attrs.HasFlag(TypeAttributes.BeforeFieldInit).ShouldBeTrue();
        attrs.HasFlag(TypeAttributes.Abstract).ShouldBeTrue();
        attrs.HasFlag(TypeAttributes.Sealed).ShouldBeTrue();
    }

    [Fact]
    public void Nullable_Test()
    {
        var str = string.Empty;

        var type = str.GetType();

        type.IsNullableType().ShouldBeTrue();

        Nullable.GetUnderlyingType(typeof(int?)).ShouldBe(typeof(int));
    }

    [Fact]
    public void Type_Name()
    {
        typeof(ParameterLessModel).Name.ShouldBe(nameof(ParameterLessModel));

        typeof(ParameterLessModel).FullName.ShouldBe("System.Type_Test+ParameterLessModel");

        typeof(ParameterLessModel).ToString().ShouldBe("System.Type_Test+ParameterLessModel");

        typeof(ParameterLessModel).GetDisplayName(false).ShouldBe(nameof(ParameterLessModel));

        typeof(int[]).GetDisplayName().ShouldBe("int[]");

        typeof(Func<int, int, int, int, int>).GetDisplayName().ShouldBe("System.Func<int, int, int, int, int>");

        typeof(Func<int, int, int, int, int>).GetDisplayName(false).ShouldBe("Func<int, int, int, int, int>");

        typeof(StaticClass).GetDisplayName().ShouldBe("System.Type_Test+StaticClass");

        typeof(StaticClass).GetDisplayName(isCompilable: true).ShouldBe("System.Type_Test.StaticClass");
    }

    [Fact]
    public void Get_base_types()
    {
        typeof(ThriedClass).IsAssignableTo<ThriedClass>().ShouldBeTrue();

        typeof(ThriedClass).IsAssignableTo<BaseClass>().ShouldBeTrue();

        typeof(ThriedClass).IsAssignableTo<IBaseInterface>().ShouldBeTrue();

        typeof(ThriedClass).IsAssignableToType(typeof(DerivedClass<>)).ShouldBeTrue();

        typeof(FourthClass<>).IsAssignableToType(typeof(DerivedClass<>)).ShouldBeTrue();
    }

    /// <summary>
    /// 是否原始类型（bool, byte, sbyte, ushort, short, uint, int, ulong, long, char, float, double）
    /// </summary>
    [Fact]
    public void Is_primitive()
    {
        // true
        typeof(bool).IsPrimitive.ShouldBeTrue();
        typeof(sbyte).IsPrimitive.ShouldBeTrue();
        typeof(byte).IsPrimitive.ShouldBeTrue();
        typeof(char).IsPrimitive.ShouldBeTrue();
        typeof(ushort).IsPrimitive.ShouldBeTrue();
        typeof(short).IsPrimitive.ShouldBeTrue();
        typeof(uint).IsPrimitive.ShouldBeTrue();
        typeof(int).IsPrimitive.ShouldBeTrue();
        typeof(ulong).IsPrimitive.ShouldBeTrue();
        typeof(long).IsPrimitive.ShouldBeTrue();
        typeof(float).IsPrimitive.ShouldBeTrue();
        typeof(double).IsPrimitive.ShouldBeTrue();

        // false
        typeof(uint?).IsPrimitive.ShouldBeFalse();
        typeof(string).IsPrimitive.ShouldBeFalse();
        typeof(decimal).IsPrimitive.ShouldBeFalse();
        typeof(Guid).IsPrimitive.ShouldBeFalse();
        typeof(DateTime).IsPrimitive.ShouldBeFalse();
    }

    [Fact]
    public void Type_display_name()
    {
        typeof(Type).Name.ShouldBe("Type");
        typeof(Type).FullName.ShouldBe("System.Type");
        typeof(Type).AssemblyQualifiedName.ShouldContain("System.Type, System.Private.CoreLib");
        typeof(Type).GetDisplayName().ShouldBe("System.Type");
        typeof(Type).GetShortDisplayName().ShouldBe("Type");

        typeof(IEnumerable<Type>).Name.ShouldBe("IEnumerable`1");
        typeof(IEnumerable<Type>).FullName.ShouldContain("System.Collections.Generic.IEnumerable`1[[System.Type, System.Private.CoreLib");
        typeof(IEnumerable<Type>).AssemblyQualifiedName.ShouldContain("System.Collections.Generic.IEnumerable`1[[System.Type, System.Private.CoreLib");
        typeof(IEnumerable<Type>).GetDisplayName().ShouldBe("System.Collections.Generic.IEnumerable<System.Type>");
        typeof(IEnumerable<Type>).GetShortDisplayName().ShouldBe("IEnumerable<Type>");

        typeof(Type[]).Name.ShouldBe("Type[]");
        typeof(Type[]).FullName.ShouldBe("System.Type[]");
        typeof(Type[]).AssemblyQualifiedName.ShouldContain("System.Type[], System.Private.CoreLib");
        typeof(Type[]).GetDisplayName().ShouldBe("System.Type[]");
        typeof(Type[]).GetShortDisplayName().ShouldBe("Type[]");

        typeof((string, int)).Name.ShouldBe("ValueTuple`2");
        typeof((string, int)).FullName.ShouldContain("System.ValueTuple`2[[System.String, System.Private.CoreLib");
        typeof((string, int)).AssemblyQualifiedName.ShouldContain("System.ValueTuple`2[[System.String, System.Private.CoreLib");
        typeof((string, int)).GetDisplayName().ShouldBe("System.ValueTuple<string, int>");
        typeof((string, int)).GetShortDisplayName().ShouldBe("ValueTuple<string, int>");
    }

    private interface IBaseInterface
    {
    }

    private class BaseClass : IBaseInterface
    {
    }

    private class DerivedClass<T> : BaseClass
    {
        public T Data { get; set; }
    }

    private class ThriedClass : DerivedClass<string>
    {
    }

    private class FourthClass<T> : DerivedClass<T>
    {
    }

    private class ParameterLessModel
    {
        private ParameterLessModel()
        { }
    }

    private static class StaticClass
    {
    }
}
