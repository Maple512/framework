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

namespace System.Linq.Expressions;

using System;
using Shouldly;
using Xunit;

public class LambdaExpression_Test
{
    [Fact]
    public LambdaExpression Build_Lambda_Exression()
    {
        var parameter = Expression.Parameter(typeof(Model), "x");

        var proparty = Expression.Property(parameter, nameof(Model.IsDeleted));

        var body = Expression.Equal(proparty, Expression.Constant(true));

        var expression = Expression.Lambda(body, parameter);

        expression.ToString().ShouldBe("x => (x.IsDeleted == True)");

        return expression;
    }

    [Fact]
    public void Call_Lambda_Expression()
    {
        var expression = Build_Lambda_Exression();

        var model = new Model { IsDeleted = true };

        ((Func<Model, bool>)expression.Compile()).Invoke(model).ShouldBeTrue();
    }

    private class Model
    {
        public bool IsDeleted { get; set; }
    }
}
