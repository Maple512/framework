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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

[StackTraceHidden]
[DebuggerStepThrough]
public static class Check
{
    [return: NotNull]
    public static string NotNullOrEmpty(
        string? value,
        string? errorMsg = null,
        [CallerArgumentExpression("value")] string? argumentExpression = null)
    {
        return string.IsNullOrEmpty(value) ? throw new ArgumentNullException(argumentExpression, errorMsg) : value;
    }

    [return: NotNull]
    public static string NotNullOrWhiteSpace(
        string? value,
        string? errorMsg = null,
        [CallerArgumentExpression("value")] string? argumentExpression = null)
    {
        return string.IsNullOrWhiteSpace(value) ? throw new ArgumentNullException(argumentExpression, errorMsg) : value;
    }

    [return: NotNull]
    public static T NotNull<T>(
        T? value,
        string? errorMsg = null,
        [CallerArgumentExpression("value")] string? argumentExpression = null)
    {
        return value == null ? throw new ArgumentNullException(argumentExpression, errorMsg) : value;
    }

    [return: NotNull]
    public static IEnumerable<T> NotNullOrEmpty<T>(
        IEnumerable<T>? data,
        string? errorMsg = null,
        [CallerArgumentExpression("data")] string? argumentExpression = null)
    {
        return data?.Any() != true ? throw new ArgumentNullException(argumentExpression, errorMsg) : data;
    }

    /// <summary>
    /// 判断给定的<paramref name="data"/>是否是一个null或空集合
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="data"></param>
    /// <param name="errorMsg"></param>
    /// <param name="argumentExpression"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    [return: NotNull]
    public static Dictionary<TKey, TValue> NotNullOrEmpty<TKey, TValue>(
        Dictionary<TKey, TValue>? data,
        string? errorMsg = null,
        [CallerArgumentExpression("data")] string? argumentExpression = null)
        where TKey : notnull
    {
        return data == null || !data.Any() ? throw new ArgumentNullException(argumentExpression, errorMsg) : data;
    }

    /// <summary>
    /// 传入的表达式是否为 <see langword="true"/>
    /// </summary>
    /// <param name="condition"></param>
    /// <param name="message"></param>
    /// <param name="argumentExpression"></param>
    /// <exception cref="ArgumentException"></exception>
    [Conditional("DEBUG")]
    public static void DebugAssert(
        [DoesNotReturnIf(false)] bool condition,
        string? message = null,
        [CallerArgumentExpression("condition")] string? argumentExpression = null)
    {
        if(!condition)
        {
            throw new ArgumentException($"Check assert failed, expression: {argumentExpression}, error: {message}");
        }
    }

    /// <summary>
    /// 判断指定的<paramref name="item"/>是否在<paramref name="data"/>中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool IsIn<T>([NotNullWhen(true)] T? item, params T[] data)
    {
        return item != null && data.Length != 0 && data.Contains(item);
    }

    /// <summary>
    /// 验证
    /// <para>如果验证失败，则抛出异常</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="validateAllProperties"><see langword="true"/>时，检查所有属性，否则，仅检查标注了<see cref="RequiredAttribute"/>的属性，默认<see langword="true"/></param>
    /// <returns></returns>
    /// <exception cref="AggregateException"></exception>
    [return: NotNullIfNotNull("instance")]
    public static T Validate<T>([NotNullWhen(true)] T instance, bool validateAllProperties = true)
        where T : class
    {
        var result = TryValidate(instance, validateAllProperties);

        if(!result.IsValid)
        {
            throw new AggregateException(result.Errors!.Select(x => new ValidationException(
                x,
                null,
                instance)));
        }

        return instance;
    }

    /// <summary>
    /// 验证
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance"></param>
    /// <param name="validateAllProperties"><see langword="true"/>时，检查所有属性，否则，仅检查标注了<see cref="RequiredAttribute"/>的属性，默认<see langword="true"/></param>
    /// <returns></returns>
    public static CheckResult TryValidate<T>(T instance, bool validateAllProperties = true)
        where T : class
    {
        _ = NotNull(instance);

        var errors = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            instance!,
            new(instance!, null, null),
            errors,
            validateAllProperties);

        return new CheckResult(isValid, errors);
    }
}
