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

using System.Collections.Generic;
using System.Diagnostics;

[StackTraceHidden]
[DebuggerStepThrough]
public static class OneFExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        return CombineLambdas(left, right, ExpressionType.AndAlso);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        return CombineLambdas(left, right, ExpressionType.OrElse);
    }

    public static Expression<Func<T, bool>> CombineLambdas<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right,
        ExpressionType expressionType)
    {
        var visitor = new SubstituteParameterVisitor
        {
            Sub =
            {
                [right.Parameters[0]] = left.Parameters[0],
            },
        };

        var body = Expression.MakeBinary(expressionType, left.Body, visitor.Visit(right.Body));

        return Expression.Lambda<Func<T, bool>>(body, left.Parameters[0]);
    }
}

internal class SubstituteParameterVisitor : ExpressionVisitor
{
    public Dictionary<Expression, Expression> Sub = new();

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return Sub.TryGetValue(node, out var newValue) ? newValue : node;
    }
}
