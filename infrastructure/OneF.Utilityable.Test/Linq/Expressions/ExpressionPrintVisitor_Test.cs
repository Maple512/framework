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

namespace OneF.Linq.Expressions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using Shouldly;
using Xunit;

public class ExpressionPrintVisitor_Test
{
    private readonly ExpressionPrintVisitor _visitor = new();

    [Fact]
    public void Unary_Expression()
    {
        _visitor.Print(Expression.Convert(Expression.Constant(42), typeof(decimal)))
            .ShouldBe("(decimal)42");

        _visitor.Print(Expression.Throw(Expression.Constant("Some exception")))
            .ShouldBe("throw \"Some exception\"");

        _visitor.Print(Expression.Not(Expression.Constant(true)))
            .ShouldBe("!(True)");

        _visitor.Print(Expression.TypeAs(Expression.Constant(1), typeof(string)))
            .ShouldBe("(1 as string)");
    }

    [Fact]
    public void Binary_Expression()
    {
        _visitor.Print(Expression.MakeBinary(ExpressionType.Equal, Expression.Constant(47), Expression.Constant(3)))
            .ShouldBe("47 == 3");
    }

    [Fact]
    public void Conditional_Expression()
    {
        _visitor.Print(Expression.Condition(Expression.Constant(true), Expression.Constant("a"), Expression.Constant("b")))
             .ShouldBe("True ? \"a\" : \"b\"");
    }

    [Fact]
    public void Simple_Lambda()
    {
        _visitor.Print(Expression.Lambda(Expression.Parameter(typeof(string), "str"), Expression.Parameter(typeof(string), "str")))
            .ShouldBe("str => str");

        _visitor.Print(
            Expression.Lambda(
                Expression.Constant(42),
                Expression.Parameter(typeof(int),
                "x")))
            .ShouldBe("x => 42");

        _visitor.Print(
            Expression.Lambda(
                Expression.Constant(42),
                Expression.Parameter(typeof(int), "a"), Expression.Parameter(typeof(int), "b")))
            .ShouldBe("(a, b) => 42");
    }

    [Fact]
    public void Block_test()
    {
        var p = Expression.Parameter(typeof(int));
        var p1 = Expression.Parameter(typeof(string));
        var end = Expression.Label();
        var lala = Expression.Label();
        var hello = Expression.Label();
        var block = Expression.Block(
            new[] { p1 },
            Expression.Switch(
                p,
                Expression.Block(
                    Expression.Assign(p1, Expression.Constant("default")),
                    Expression.Goto(end)
                    ),
                Expression.SwitchCase(Expression.Goto(hello), Expression.Constant(1)),
                Expression.SwitchCase(Expression.Block(
                    Expression.Assign(p1, Expression.Constant("two")),
                    Expression.Goto(end)
                    ), Expression.Constant(2)),
                Expression.SwitchCase(Expression.Goto(lala), Expression.Constant(4))
                ),
            Expression.Label(hello),
            Expression.Assign(p1, Expression.Constant("hello")),
            Expression.Goto(end),
            Expression.Label(lala),
            Expression.Assign(p1, Expression.Constant("lala")),
            Expression.Label(end),
            p1
            );
        // https://github.com/sprigodaENDV/Native-AOT-Full-Copy/blob/460890057bde4e58182b9e7121828565d36fcad9/src/libraries/System.Linq.Expressions/tests/Goto/Break.cs
        var expression = Expression.Lambda<Func<int, string>>(block, p);

        var f = expression.Compile();

        Assert.Equal("hello", f(1));
        Assert.Equal("two", f(2));
        Assert.Equal("default", f(3));
        Assert.Equal("lala", f(4));
    }

    [Fact]
    public void Switch_case()
    {
        var parameter = Expression.Parameter(typeof(PrinterEnum), "value");

        var switchCases = new List<SwitchCase>(7);

        var label = Expression.Label();

        foreach(var field in typeof(PrinterEnum).GetFields(BindingFlagExtensions.EnumFlags))
        {
            var switchCase = Expression.SwitchCase(
               Expression.Constant(field.Name),
                Expression.Constant(Enum.ToObject(typeof(PrinterEnum), field.GetValue(null)!)));

            switchCases.Add(switchCase);
        }

        var body = Expression.Switch(parameter, Expression.Call(parameter, typeof(object).GetMethod(nameof(ToString))!), null, switchCases.ToArray());

        var lamdba = Expression.Lambda(body, new[] { parameter });

        /*
         value => switch (value)
            {
                case A:
                    ""A""
                case B:
                    ""B""
                case C:
                    ""C""
                case D:
                    ""D""
                case E:
                    ""E""
                case F:
                    ""F""
                case G:
                    ""G""
                default:
                    value.ToString()
            }
         */

        var result = _visitor.Print(lamdba);

        result.ShouldStartWith("value => switch (value)");

        Debug.Print(result);
    }

    internal enum PrinterEnum
    {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6,
        G = 7,
    }
}
