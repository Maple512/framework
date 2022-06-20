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
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

public class Expression_Test
{
    [Fact]
    public void Combine_Lambdas()
    {
        var count = 10;

        var list = Enumerable.Range(0, count)
                             .Select(x => new Model(x));

        Expression<Func<Model, bool>> left = x => x.Id > 2;
        Expression<Func<Model, bool>> right = x => x.Id < 5;

        // and
        list.Where(left.And(right).Compile()).GetCount().ShouldBe(2);

        // or
        list.Where(left.Or(right).Compile()).GetCount().ShouldBe(count);
    }

    [Fact]
    public void Create_Type_From_Private_Parameterless_Constructor()
    {
        var acrivator = Expression.Lambda<Func<Model>>(Expression.New(typeof(Model))).Compile();

        var model = acrivator();

        _ = model.ShouldNotBeNull();
    }

    private class Model
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
}
