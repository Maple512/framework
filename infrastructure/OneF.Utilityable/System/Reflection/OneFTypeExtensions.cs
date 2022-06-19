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

namespace System.Reflection;

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using OneF;

[StackTraceHidden]
[DebuggerStepThrough]
public static class OneFTypeExtensions
{
    #region Attribute

    public static bool IsDefined<T>(this MemberInfo member, bool inherit = false)
    {
        return member.IsDefined(typeof(T), inherit);
    }

    public static T GetRequiredAttribute<T>(this Type type)
        where T : Attribute
    {
        var attribute = type.GetCustomAttribute<T>();

        return Check.NotNull(attribute, $"Could not find attribute '{typeof(T)}' on type '{type}'");
    }

    #endregion Attribute

    #region Method

    /// <summary>
    /// 获取静态泛型方法
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <param name="parametersCount"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static MethodInfo GetStaticGenericMethod(this Type type, string methodName, int parametersCount)
        => type.GetMethods(BindingFlagExtensions.StaticFlags & ~BindingFlags.NonPublic)
            .FirstOrDefault(x => x.Name == methodName && x.IsGenericMethod && x.GetParameters().Length == parametersCount)
            ?? throw new ArgumentOutOfRangeException(nameof(methodName), $"Cannot find suitable method {type.GetDisplayName()}.{methodName}({parametersCount} parameters).");

    public static MethodInfo GetRequiredDeclaredMethod(this Type type, string name)
    {
        var method = type.GetTypeInfo().GetDeclaredMethod(name);

        return Check.NotNull(method, $"Could not find method '{name}' on type '{type}'");
    }

    #endregion Method

    #region Type

    /// <summary>
    /// 内置类型名称
    /// </summary>
    private static readonly Dictionary<Type, string> _builtInTypeNames = new()
    {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(long), "long" },
        { typeof(object), "object" },
        { typeof(sbyte), "sbyte" },
        { typeof(short), "short" },
        { typeof(string), "string" },
        { typeof(uint), "uint" },
        { typeof(ulong), "ulong" },
        { typeof(ushort), "ushort" },
        { typeof(void), "void" }
    };

    /// <inheritdoc cref="Type.IsAssignableTo(Type?)"/>
    public static bool IsAssignableTo<TBase>(this Type type)
    {
        return type.IsAssignableTo(typeof(TBase));
    }

    /// <summary>
    /// 判断 <typeparamref name="TBase"/> 是否 <paramref name="type"/> 的基类（包括泛型基类）
    /// </summary>
    /// <typeparam name="TBase"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAssignableToType<TBase>(this Type type)
    {
        return IsAssignableToType(type, typeof(TBase));
    }

    /// <summary>
    /// 判断 <paramref name="baseType"/> 是否 <paramref name="type"/> 的基类（包括泛型基类）
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseType"></param>
    /// <returns></returns>
    public static bool IsAssignableToType(this Type type, Type baseType)
    {
        if(type.IsAssignableTo(baseType))
        {
            return true;
        }

        if(type.IsGenericType && type.GetGenericTypeDefinition() == baseType)
        {
            return true;
        }

        foreach(var interfaceType in type.GetInterfaces())
        {
            if(interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == baseType)
            {
                return true;
            }
        }

        if(type.BaseType == null)
        {
            return false;
        }

        return IsAssignableToType(type.BaseType, baseType);
    }

    /// <summary>
    /// 判断 <paramref name="baseTypes"/> 是否 <paramref name="type"/> 的基类（包括泛型基类）
    /// </summary>
    /// <param name="type"></param>
    /// <param name="baseTypes"></param>
    /// <returns></returns>
    public static bool IsAssignableToTypes(this Type type, params Type[] baseTypes)
    {
        if(baseTypes.Length == 0)
        {
            return true;
        }

        foreach(var baseType in baseTypes)
        {
            if(IsAssignableToType(type, baseType))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc cref="Type.IsAssignableFrom(Type?)"/>
    public static bool IsAssignableFrom<T>(this Type baesType)
    {
        return baesType.IsAssignableFrom(typeof(T));
    }

    // TODO: 待测试
    /// <summary>
    /// 是否基元类型（int, uint, sbyte, byte ...）
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsRichPrimitive(this Type type)
    {
        // 处理元组类型
        if(type.IsValueType)
        {
            return false;
        }

        // 处理数组类型，基元数组类型也可以是基元类型
        if(type.IsArray)
        {
            return type.GetElementType()!.IsRichPrimitive();
        }

        // 基元类型或值类型或字符串类型
        if(type.IsPrimitive || type.IsValueType || type == typeof(string))
        {
            return true;
        }

        if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return type.GenericTypeArguments[0].IsRichPrimitive();
        }

        return false;
    }

    /// <summary>
    /// 可空的值类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullableValueType(this Type type)
            => type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>
    /// 可空的类型（引用类型都是可空的，值类型需要另外的判断）
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullableType(this Type type)
        => !type.IsValueType || type.IsNullableValueType();

    /// <summary>
    /// 去掉可空类型的包装。 <c>int?</c>-&gt; <c>int</c>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type UnwrapNullableType(this Type type)
            => Nullable.GetUnderlyingType(type) ?? type;

    /// <summary>
    /// 转换为人性化可读的名称
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fullName"></param>
    /// <param name="isCompilable"></param>
    /// <returns></returns>
    public static string GetDisplayName(this Type type, bool fullName = true, bool isCompilable = false)
    {
        var stringBuilder = new StringBuilder();

        try
        {
            ProcessType(stringBuilder, type, fullName, isCompilable);

            return stringBuilder.ToString();
        }
        finally
        {
            _ = stringBuilder.Clear();
        }
    }

    /// <summary>
    /// 转换为人性化可读的名称
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetShortDisplayName(this Type type) => type.GetDisplayName(false);

    private static void ProcessType(StringBuilder builder, Type type, bool fullName, bool isCompilable)
    {
        if(type.IsGenericType)
        {
            var genericArguments = type.GetGenericArguments();
            ProcessGenericType(builder, type, genericArguments, genericArguments.Length, fullName, isCompilable);
        }
        else if(type.IsArray)
        {
            ProcessArrayType(builder, type, fullName, isCompilable);
        }
        else if(_builtInTypeNames.TryGetValue(type, out var builtInName))
        {
            _ = builder.Append(builtInName);
        }
        else if(!type.IsGenericParameter)
        {
            if(isCompilable)
            {
                if(type.IsNested)
                {
                    ProcessType(builder, type.DeclaringType!, fullName, isCompilable);
                    _ = builder.Append('.');
                }
                else if(fullName)
                {
                    _ = builder.Append(type.Namespace).Append('.');
                }

                _ = builder.Append(type.Name);
            }
            else
            {
                _ = builder.Append(fullName ? type.FullName : type.Name);
            }
        }
    }

    private static void ProcessArrayType(StringBuilder builder, Type type, bool fullName, bool isCompilable)
    {
        var innerType = type;
        while(innerType.IsArray)
        {
            innerType = innerType.GetElementType()!;
        }

        ProcessType(builder, innerType, fullName, isCompilable);

        while(type.IsArray)
        {
            _ = builder.Append('[');
            _ = builder.Append(',', type.GetArrayRank() - 1);
            _ = builder.Append(']');
            type = type.GetElementType()!;
        }
    }

    private static void ProcessGenericType(
        StringBuilder builder,
        Type type,
        Type[] genericArguments,
        int length,
        bool fullName,
        bool isCompilable)
    {
        if(type.IsConstructedGenericType
            && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            ProcessType(builder, type.UnwrapNullableType(), fullName, isCompilable);
            _ = builder.Append('?');
            return;
        }

        var offset = type.IsNested ? type.DeclaringType!.GetGenericArguments().Length : 0;

        if(isCompilable)
        {
            if(type.IsNested)
            {
                ProcessType(builder, type.DeclaringType!, fullName, isCompilable);
                _ = builder.Append('.');
            }
            else if(fullName)
            {
                _ = builder.Append(type.Namespace);
                _ = builder.Append('.');
            }
        }
        else
        {
            if(fullName)
            {
                if(type.IsNested)
                {
                    ProcessGenericType(builder, type.DeclaringType!, genericArguments, offset, fullName, isCompilable);
                    _ = builder.Append('+');
                }
                else
                {
                    _ = builder.Append(type.Namespace);
                    _ = builder.Append('.');
                }
            }
        }

        var genericPartIndex = type.Name.IndexOf('`');
        if(genericPartIndex <= 0)
        {
            _ = builder.Append(type.Name);
            return;
        }

        _ = builder.Append(type.Name, 0, genericPartIndex);
        _ = builder.Append('<');

        for(var i = offset; i < length; i++)
        {
            ProcessType(builder, genericArguments[i], fullName, isCompilable);
            if(i + 1 == length)
            {
                continue;
            }

            _ = builder.Append(',');
            if(!genericArguments[i + 1].IsGenericParameter)
            {
                _ = builder.Append(' ');
            }
        }

        _ = builder.Append('>');
    }

    /// <summary>
    /// <para>判断指定的类型是否数字</para>
    /// <para>数字：整数<see cref="IsInteger"/>, <see cref="decimal"/>, <see cref="float"/>, <see cref="double"/></para>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNumeric(this Type type)
    {
        return NumericTypes.Contains(type.UnwrapNullableType());
    }

    /// <summary>
    /// 数字：<see cref="sbyte"/>, <see cref="byte"/>, <see cref="char"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>, <see cref="int"/>, <see cref="ulong"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, <see cref="decimal"/>
    /// </summary>
    public static readonly IEnumerable<Type> NumericTypes = new[]
    {
        typeof(sbyte),
        typeof(byte),
        typeof(char),
        typeof(ushort),
        typeof(short),
        typeof(uint),
        typeof(int),
        typeof(ulong),
        typeof(long),
        typeof(float),
        typeof(double),
        typeof(decimal),
    };

    /// <summary>
    /// 整数：<see cref="sbyte"/>, <see cref="byte"/>, <see cref="char"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>, <see cref="int"/>, <see cref="ulong"/>, <see cref="long"/>
    /// </summary>
    public static readonly IEnumerable<Type> IntegerTypes = new[]
    {
        typeof(sbyte),
        typeof(byte),
        typeof(char),
        typeof(ushort),
        typeof(short),
        typeof(uint),
        typeof(int),
        typeof(ulong),
        typeof(long),
    };

    /// <summary>
    /// <para>判断指定的类型是否整数</para>
    /// <para>整数：<see cref="sbyte"/>, <see cref="byte"/>, <see cref="char"/>, <see cref="ushort"/>, <see cref="short"/>, <see cref="uint"/>, <see cref="int"/>, <see cref="ulong"/>, <see cref="long"/></para>
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsInteger(this Type type)
    {
        return IntegerTypes.Contains(type.UnwrapNullableType());
    }

    /// <summary>
    /// 有符号整数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSignedInteger(this Type type)
        => type == typeof(int)
            || type == typeof(long)
            || type == typeof(short)
            || type == typeof(sbyte);

    /// <summary>
    /// 是否匿名类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAnonymousType(this Type type)
        => type.Name.StartsWith("<>", StringComparison.Ordinal)
            && type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), inherit: false).Length > 0
            && type.Name.Contains("AnonymousType");

    #endregion Type

    #region Constructor

    /// <summary>
    /// 尝试获取无参构造器
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool TryGetParameterlessConstructor(this Type type, [NotNullWhen(true)] out ConstructorInfo? constructor)
        => (constructor = type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(x => x.GetParameters().IsNullOrEmpty())) != null;

    #endregion Constructor
}
