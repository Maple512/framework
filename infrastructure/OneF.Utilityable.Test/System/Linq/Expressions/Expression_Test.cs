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
using System.Linq;
using Shouldly;
using Xunit;

public class Expression_Test
{
    [Fact]
    public void Combine_Lambdas()
    {
        var list = Enumerable.Range(1, 10)
                             .Select(x => new Model(x));

        Expression<Func<Model, bool>> left = x => x.Id > 2;
        Expression<Func<Model, bool>> right = x => x.Id < 5;

        var result = list.Where(left.And(right).Compile());

        Assert.True(result.Count() == 2);
    }

    [Fact]
    public void Create_Type_From_Private_Parameterless_Constructor()
    {
        var acrivator = Expression.Lambda<Func<Model>>(Expression.New(typeof(Model))).Compile();

        var model = acrivator();

        _ = model.ShouldNotBeNull();
    }
}

internal class Model
{
    private Model()
    {
    }

    public Model(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
