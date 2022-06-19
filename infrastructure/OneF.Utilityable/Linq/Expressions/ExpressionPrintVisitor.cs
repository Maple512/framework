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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using OneF.Text;

public class ExpressionPrintVisitor : ExpressionVisitor
{
    private readonly IndentedStringBuilder _stringBuilder;
    private readonly Dictionary<ParameterExpression, string?> _parametersInScope;
    private readonly List<ParameterExpression> _namelessParameters;
    private readonly List<ParameterExpression> _encounteredParameters;

    public ExpressionPrintVisitor()
    {
        _stringBuilder = new();
        _parametersInScope = new();
        _namelessParameters = new();
        _encounteredParameters = new();
    }

    private int? CharacterLimit { get; set; }
    private bool Verbose { get; set; }

    private readonly Dictionary<ExpressionType, string> _binaryOperandMap = new()
    {
        { ExpressionType.Assign, " = " },
        { ExpressionType.Equal, " == " },
        { ExpressionType.NotEqual, " != " },
        { ExpressionType.GreaterThan, " > " },
        { ExpressionType.GreaterThanOrEqual, " >= " },
        { ExpressionType.LessThan, " < " },
        { ExpressionType.LessThanOrEqual, " <= " },
        { ExpressionType.OrElse, " || " },
        { ExpressionType.AndAlso, " && " },
        { ExpressionType.Coalesce, " ?? " },
        { ExpressionType.Add, " + " },
        { ExpressionType.Subtract, " - " },
        { ExpressionType.Multiply, " * " },
        { ExpressionType.Divide, " / " },
        { ExpressionType.Modulo, " % " },
        { ExpressionType.And, " & " },
        { ExpressionType.Or, " | " },
        { ExpressionType.ExclusiveOr, " ^ " }
    };

    private static readonly List<string> _simpleMethods = new()
    {
        "get_Item",
        "TryReadValue",
        "ReferenceEquals"
    };

    #region Append String

    public virtual ExpressionPrintVisitor AppendLine()
    {
        _ = _stringBuilder.AppendLine();

        return this;
    }

    public virtual ExpressionVisitor AppendLine(string value)
    {
        _ = _stringBuilder.AppendLine(value);

        return this;
    }

    public virtual ExpressionPrintVisitor AppendLines(string value, bool skipFinalNewline = false)
    {
        _ = _stringBuilder.AppendLines(value, skipFinalNewline);

        return this;
    }

    public virtual IDisposable Indent() => _stringBuilder.Indent();

    public virtual ExpressionPrintVisitor Append(string value)
    {
        _ = _stringBuilder.Append(value);

        return this;
    }

    #endregion Append String

    public virtual string Print(
            Expression expression,
            int? characterLimit = null)
            => PrintCore(expression, characterLimit, false);

    public virtual string PrintDebug(
            Expression expression)
            => PrintCore(expression, null, true);

    /// <summary>
    /// 打印给定的表达式
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="characterLimit">字符数的长度限制，超过部分将被截断</param>
    /// <param name="verbose">是否打印非常详细的信息</param>
    /// <returns></returns>
    private string PrintCore(
        Expression expression,
        int? characterLimit,
        bool verbose)
    {
        _ = _stringBuilder.Clear();
        _parametersInScope.Clear();
        _namelessParameters.Clear();
        _encounteredParameters.Clear();

        CharacterLimit = characterLimit;
        Verbose = verbose;

        _ = Visit(expression);

        // 后期处理
        var queryPlan = PostProcess(_stringBuilder.ToString());

        if(characterLimit != null
                && characterLimit.Value > 0)
        {
            queryPlan = queryPlan.Length > characterLimit
                ? queryPlan[..characterLimit.Value] + "..."
                : queryPlan;
        }

        return queryPlan;
    }

    #region Visitor

    protected override Expression VisitBinary(BinaryExpression node)
    {
        _ = Visit(node.Left);

        if(node.NodeType == ExpressionType.ArrayIndex)
        {
            _ = _stringBuilder.Append("[");

            _ = Visit(node.Right);

            _ = _stringBuilder.Append("]");
        }
        else
        {
            if(_binaryOperandMap.TryGetValue(node.NodeType, out var binaryOperand))
            {
                _ = _stringBuilder.Append(binaryOperand);
            }
            else
            {
                UnhandledExpressionType(node);
            }

            _ = Visit(node.Right);
        }

        return node;
    }

    protected override Expression VisitBlock(BlockExpression node)
    {
        _ = AppendLine();
        _ = AppendLine("{");

        using(_stringBuilder.Indent())
        {
            foreach(var variable in node.Variables)
            {
                if(!_parametersInScope.ContainsKey(variable))
                {
                    _parametersInScope.Add(variable, variable.Name);

                    _ = Append(variable.Type.GetShortDisplayName());

                    _ = Append(" ");

                    _ = VisitParameter(variable);

                    _ = AppendLine(";");
                }
            }

            var expressions = node.Result != null
                ? node.Expressions.Except(new[] { node.Result })
                : node.Expressions;

            foreach(var expression in expressions)
            {
                _ = Visit(expression);

                _ = AppendLine(";");
            }

            if(node.Result != null)
            {
                _ = Append("return ");

                _ = Visit(node.Result);

                _ = AppendLine(";");
            }
        }

        _ = Append("}");

        return base.VisitBlock(node);
    }

    protected override Expression VisitConditional(ConditionalExpression node)
    {
        _ = Visit(node.Test);

        _ = _stringBuilder.Append(" ? ");

        _ = Visit(node.IfTrue);

        _ = _stringBuilder.Append(" : ");

        _ = Visit(node.IfFalse);

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression constantExpression)
    {
        if(constantExpression.Value is IPrintableExpression printable)
        {
            printable.Print(this);
        }
        else
        {
            Print(constantExpression.Value);
        }

        return constantExpression;
    }

    protected override Expression VisitGoto(GotoExpression gotoExpression)
    {
        _ = AppendLine("return (" + gotoExpression.Target.Type.GetShortDisplayName() + ")" + gotoExpression.Target + " {");
        using(_stringBuilder.Indent())
        {
            _ = Visit(gotoExpression.Value);
        }

        _ = _stringBuilder.Append("}");

        return gotoExpression;
    }

    protected override Expression VisitLabel(LabelExpression labelExpression)
    {
        _ = _stringBuilder.Append(labelExpression.Target.ToString());

        return labelExpression;
    }

    protected override Expression VisitLambda<T>(Expression<T> lambdaExpression)
    {
        if(lambdaExpression.Parameters.Count != 1)
        {
            _ = _stringBuilder.Append("(");
        }

        foreach(var parameter in lambdaExpression.Parameters)
        {
            var parameterName = parameter.Name;

            if(!_parametersInScope.ContainsKey(parameter))
            {
                _parametersInScope.Add(parameter, parameterName);
            }

            _ = Visit(parameter);

            if(parameter != lambdaExpression.Parameters.Last())
            {
                _ = _stringBuilder.Append(", ");
            }
        }

        if(lambdaExpression.Parameters.Count != 1)
        {
            _ = _stringBuilder.Append(")");
        }

        _ = _stringBuilder.Append(" => ");

        _ = Visit(lambdaExpression.Body);

        foreach(var parameter in lambdaExpression.Parameters)
        {
            // however we don't remove nameless parameters so that they are unique globally, not
            // just within the scope
            _ = _parametersInScope.Remove(parameter);
        }

        return lambdaExpression;
    }

    protected override Expression VisitMember(MemberExpression memberExpression)
    {
        if(memberExpression.Expression != null)
        {
            if(memberExpression.Expression.NodeType == ExpressionType.Convert
                || memberExpression.Expression is BinaryExpression)
            {
                _ = _stringBuilder.Append("(");
                _ = Visit(memberExpression.Expression);
                _ = _stringBuilder.Append(")");
            }
            else
            {
                _ = Visit(memberExpression.Expression);
            }
        }
        else
        {
            // ReSharper disable once PossibleNullReferenceException
            _ = _stringBuilder.Append(memberExpression.Member.DeclaringType?.Name ?? "MethodWithoutDeclaringType");
        }

        _ = _stringBuilder.Append("." + memberExpression.Member.Name);

        return memberExpression;
    }

    protected override Expression VisitMemberInit(MemberInitExpression memberInitExpression)
    {
        _ = _stringBuilder.Append("new " + memberInitExpression.Type.GetShortDisplayName());

        var appendAction = memberInitExpression.Bindings.Count > 1 ? (Func<string, ExpressionVisitor>)AppendLine : Append;
        _ = appendAction("{ ");
        using(_stringBuilder.Indent())
        {
            for(var i = 0; i < memberInitExpression.Bindings.Count; i++)
            {
                var binding = memberInitExpression.Bindings[i];
                if(binding is MemberAssignment assignment)
                {
                    _ = _stringBuilder.Append(assignment.Member.Name + " = ");
                    _ = Visit(assignment.Expression);
                    _ = appendAction(i == memberInitExpression.Bindings.Count - 1 ? " " : ", ");
                }
                else
                {
                    _ = AppendLine($"Unhandled member binding type '{binding.BindingType}'.");
                }
            }
        }

        _ = AppendLine("}");

        return memberInitExpression;
    }

    protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
    {
        if(methodCallExpression.Object != null)
        {
            if(methodCallExpression.Object is BinaryExpression)
            {
                _ = _stringBuilder.Append("(");
                _ = Visit(methodCallExpression.Object);
                _ = _stringBuilder.Append(")");
            }
            else
            {
                _ = Visit(methodCallExpression.Object);
            }

            _ = _stringBuilder.Append(".");
        }

        var methodArguments = methodCallExpression.Arguments.ToList();
        var method = methodCallExpression.Method;

        var extensionMethod = !Verbose
            && methodCallExpression.Arguments.Count > 0
            && method.IsDefined(typeof(ExtensionAttribute), inherit: false);

        if(extensionMethod)
        {
            _ = Visit(methodArguments[0]);
            _ = _stringBuilder.IncrementIndent();
            _ = _stringBuilder.AppendLine();
            _ = _stringBuilder.Append($".{method.Name}");
            methodArguments = methodArguments.Skip(1).ToList();
            if(method.Name is (nameof(Enumerable.Cast))
                or (nameof(Enumerable.OfType)))
            {
                PrintGenericArguments(method, _stringBuilder);
            }
        }
        else
        {
            if(method.IsStatic)
            {
                _ = _stringBuilder.Append(method.DeclaringType!.GetShortDisplayName()).Append(".");
            }

            _ = _stringBuilder.Append(method.Name);
            PrintGenericArguments(method, _stringBuilder);
        }

        _ = _stringBuilder.Append("(");

        var isSimpleMethodOrProperty = _simpleMethods.Contains(method.Name)
            || methodArguments.Count < 2
            //|| method.IsEFPropertyMethod()
            ;

        var appendAction = isSimpleMethodOrProperty ? (Func<string, ExpressionVisitor>)Append : AppendLine;

        if(methodArguments.Count > 0)
        {
            _ = appendAction("");

            var argumentNames
                = !isSimpleMethodOrProperty
                    ? extensionMethod
                        ? method.GetParameters().Skip(1).Select(p => p.Name).ToList()
                        : method.GetParameters().Select(p => p.Name).ToList()
                    : new List<string?>();

            IDisposable? indent = null;

            if(!isSimpleMethodOrProperty)
            {
                indent = _stringBuilder.Indent();
            }

            for(var i = 0; i < methodArguments.Count; i++)
            {
                var argument = methodArguments[i];

                if(!isSimpleMethodOrProperty)
                {
                    _ = _stringBuilder.Append(argumentNames[i] + ": ");
                }

                _ = Visit(argument);

                if(i < methodArguments.Count - 1)
                {
                    _ = appendAction(", ");
                }
            }

            if(!isSimpleMethodOrProperty)
            {
                indent?.Dispose();
            }
        }

        _ = Append(")");

        if(extensionMethod)
        {
            _ = _stringBuilder.DecrementIndent();
        }

        return methodCallExpression;

        static void PrintGenericArguments(MethodInfo method, IndentedStringBuilder stringBuilder)
        {
            if(method.IsGenericMethod)
            {
                _ = stringBuilder.Append("<");
                var first = true;
                foreach(var genericArgument in method.GetGenericArguments())
                {
                    if(!first)
                    {
                        _ = stringBuilder.Append(", ");
                    }

                    _ = stringBuilder.Append(genericArgument.GetShortDisplayName());
                    first = false;
                }

                _ = stringBuilder.Append(">");
            }
        }
    }

    protected override Expression VisitNew(NewExpression newExpression)
    {
        _ = _stringBuilder.Append("new ");

        var isComplex = newExpression.Arguments.Count > 1;
        var appendAction = isComplex ? (Func<string, ExpressionVisitor>)AppendLine : Append;

        var isAnonymousType = newExpression.Type.IsAnonymousType();
        if(!isAnonymousType)
        {
            _ = _stringBuilder.Append(newExpression.Type.GetShortDisplayName());
            _ = appendAction("(");
        }
        else
        {
            _ = appendAction("{ ");
        }

        IDisposable? indent = null;
        if(isComplex)
        {
            indent = _stringBuilder.Indent();
        }

        for(var i = 0; i < newExpression.Arguments.Count; i++)
        {
            if(newExpression.Members != null)
            {
                _ = Append(newExpression.Members[i].Name + " = ");
            }

            _ = Visit(newExpression.Arguments[i]);
            _ = appendAction(i == newExpression.Arguments.Count - 1 ? "" : ", ");
        }

        if(isComplex)
        {
            indent?.Dispose();
        }

        if(!isAnonymousType)
        {
            _ = _stringBuilder.Append(")");
        }
        else
        {
            _ = _stringBuilder.Append(" }");
        }

        return newExpression;
    }

    protected override Expression VisitNewArray(NewArrayExpression newArrayExpression)
    {
        var isComplex = newArrayExpression.Expressions.Count > 1;
        var appendAction = isComplex ? s => AppendLine(s) : (Action<string>)(s => Append(s));

        appendAction("new " + newArrayExpression.Type.GetElementType()!.GetShortDisplayName() + "[]");
        appendAction("{ ");

        IDisposable? indent = null;
        if(isComplex)
        {
            indent = _stringBuilder.Indent();
        }

        VisitArguments(newArrayExpression.Expressions, appendAction, lastSeparator: " ");

        if(isComplex)
        {
            indent?.Dispose();
        }

        _ = Append("}");

        return newArrayExpression;
    }

    protected override Expression VisitParameter(ParameterExpression parameterExpression)
    {
        if(_parametersInScope.TryGetValue(parameterExpression, out var parameterName))
        {
            if(parameterName == null)
            {
                if(!_namelessParameters.Contains(parameterExpression))
                {
                    _namelessParameters.Add(parameterExpression);
                }

                _ = Append("namelessParameter{");
                _ = Append(_namelessParameters.IndexOf(parameterExpression).ToString());
                _ = Append("}");
            }
            else if(parameterName.Contains("."))
            {
                _ = Append("[");
                _ = Append(parameterName);
                _ = Append("]");
            }
            else
            {
                _ = Append(parameterName);
            }
        }
        else
        {
            if(Verbose)
            {
                _ = Append("(Unhandled parameter: ");
                _ = Append(parameterExpression.Name ?? "NoNameParameter");
                _ = Append(")");
            }
            else
            {
                _ = Append(parameterExpression.Name ?? "NoNameParameter");
            }
        }

        if(Verbose)
        {
            var parameterIndex = _encounteredParameters.Count;
            if(_encounteredParameters.Contains(parameterExpression))
            {
                parameterIndex = _encounteredParameters.IndexOf(parameterExpression);
            }
            else
            {
                _encounteredParameters.Add(parameterExpression);
            }

            _ = _stringBuilder.Append("{" + parameterIndex + "}");
        }

        return parameterExpression;
    }

    protected override Expression VisitUnary(UnaryExpression unaryExpression)
    {
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch(unaryExpression.NodeType)
        {
            case ExpressionType.Convert:
                _ = _stringBuilder.Append("(" + unaryExpression.Type.GetShortDisplayName() + ")");

                if(unaryExpression.Operand is BinaryExpression)
                {
                    _ = _stringBuilder.Append("(");
                    _ = Visit(unaryExpression.Operand);
                    _ = _stringBuilder.Append(")");
                }
                else
                {
                    _ = Visit(unaryExpression.Operand);
                }

                break;

            case ExpressionType.Throw:
                _ = _stringBuilder.Append("throw ");
                _ = Visit(unaryExpression.Operand);
                break;

            case ExpressionType.Not:
                _ = _stringBuilder.Append("!(");
                _ = Visit(unaryExpression.Operand);
                _ = _stringBuilder.Append(")");
                break;

            case ExpressionType.TypeAs:
                _ = _stringBuilder.Append("(");
                _ = Visit(unaryExpression.Operand);
                _ = _stringBuilder.Append(" as " + unaryExpression.Type.GetShortDisplayName() + ")");
                break;

            case ExpressionType.Quote:
                _ = Visit(unaryExpression.Operand);
                break;

            default:
                UnhandledExpressionType(unaryExpression);
                break;
        }

        return unaryExpression;
    }

    protected override Expression VisitDefault(DefaultExpression defaultExpression)
    {
        _ = _stringBuilder.Append("default(" + defaultExpression.Type.GetShortDisplayName() + ")");

        return defaultExpression;
    }

    protected override Expression VisitTry(TryExpression tryExpression)
    {
        _ = _stringBuilder.Append("try { ");
        _ = Visit(tryExpression.Body);
        _ = _stringBuilder.Append(" } ");

        foreach(var handler in tryExpression.Handlers)
        {
            _ = _stringBuilder.Append("catch (" + handler.Test.Name + ") { ... } ");
        }

        return tryExpression;
    }

    protected override Expression VisitIndex(IndexExpression indexExpression)
    {
        _ = Visit(indexExpression.Object);
        _ = _stringBuilder.Append("[");
        VisitArguments(
            indexExpression.Arguments, s => { _ = _stringBuilder.Append(s); });
        _ = _stringBuilder.Append("]");

        return indexExpression;
    }

    protected override Expression VisitTypeBinary(TypeBinaryExpression typeBinaryExpression)
    {
        _ = _stringBuilder.Append("(");
        _ = Visit(typeBinaryExpression.Expression);
        _ = _stringBuilder.Append(" is " + typeBinaryExpression.TypeOperand.GetShortDisplayName() + ")");

        return typeBinaryExpression;
    }

    protected override Expression VisitSwitch(SwitchExpression switchExpression)
    {
        _ = _stringBuilder.Append("switch (");
        _ = Visit(switchExpression.SwitchValue);
        _ = _stringBuilder.AppendLine(")");
        _ = _stringBuilder.AppendLine("{");
        _ = _stringBuilder.IncrementIndent();

        foreach(var @case in switchExpression.Cases)
        {
            foreach(var testValue in @case.TestValues)
            {
                _ = _stringBuilder.Append("case ");
                _ = Visit(testValue);
                _ = _stringBuilder.AppendLine(": ");
            }

            using(var indent = _stringBuilder.Indent())
            {
                _ = Visit(@case.Body);
            }

            _ = _stringBuilder.AppendLine();
        }

        if(switchExpression.DefaultBody != null)
        {
            _ = _stringBuilder.AppendLine("default: ");
            using(_stringBuilder.Indent())
            {
                _ = Visit(switchExpression.DefaultBody);
            }

            _ = _stringBuilder.AppendLine();
        }

        _ = _stringBuilder.DecrementIndent();
        _ = _stringBuilder.AppendLine("}");

        return switchExpression;
    }

    protected override Expression VisitExtension(Expression extensionExpression)
    {
        if(extensionExpression is IPrintableExpression printable)
        {
            printable.Print(this);
        }
        else
        {
            UnhandledExpressionType(extensionExpression);
        }

        return extensionExpression;
    }

    private void VisitArguments(
        IReadOnlyList<Expression> arguments,
        Action<string> appendAction,
        string lastSeparator = "",
        bool areConnected = false)
    {
        for(var i = 0; i < arguments.Count; i++)
        {
            if(areConnected && i == arguments.Count - 1)
            {
                _ = Append("");
            }

            _ = Visit(arguments[i]);
            appendAction(i == arguments.Count - 1 ? lastSeparator : ", ");
        }
    }

    private void Print(object? value)
    {
        if(value is IEnumerable enumerable
            and not string)
        {
            _ = _stringBuilder.Append(value.GetType().GetShortDisplayName() + " { ");

            var first = true;
            foreach(var item in enumerable)
            {
                if(first)
                {
                    first = false;
                }
                else
                {
                    _ = _stringBuilder.Append(", ");
                }

                Print(item);
            }

            _ = _stringBuilder.Append(" }");
            return;
        }

        var stringValue = value == null
            ? "null"
            : value.ToString() != value.GetType().ToString()
                ? value.ToString()
                : value.GetType().GetShortDisplayName();

        if(value is string)
        {
            stringValue = $@"""{stringValue}""";
        }

        _ = _stringBuilder.Append(stringValue ?? "Unknown");
    }

    #endregion Visitor

    private static string PostProcess(string printedExpression)
    {
        return printedExpression
            .Replace($"{Environment.NewLine}{Environment.NewLine}", Environment.NewLine);
    }

    private void UnhandledExpressionType(Expression expression)
            => AppendLine(expression.ToString());
}
