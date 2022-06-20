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

namespace System.Collections.Generic;

using System.Diagnostics;
using System.Linq;
using OneF;

[StackTraceHidden]
[DebuggerStepThrough]
public static class OneFEnumerableExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source?.Any() != true;
    }

    public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
    {
        _ = Check.NotNull(source);

        return string.Join(separator, source);
    }

    public static string JoinAsString(this IEnumerable source, char separator = ',')
    {
        _ = Check.NotNull(source);

        var list = new List<string?>();

        foreach(var item in source)
        {
            list.Add(item?.ToString());
        }

        return string.Join(separator, list);
    }

    public static string JoinAsString<T>(
        this IEnumerable<T> source,
        char separator = ',')
    {
        _ = Check.NotNull(source);

        return string.Join(separator, source);
    }

    public static IEnumerable<T> WhereIf<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> predicate)
    {
        _ = Check.NotNull(source);

        return condition ? source.Where(predicate) : source;
    }

    public static IEnumerable<T> WhereIfElse<T>(
        this IEnumerable<T> source,
        bool condition,
        Func<T, bool> @true,
        Func<T, bool> @false)
    {
        _ = Check.NotNull(source);

        return source.Where(condition ? @true : @false);
    }

    public static IEnumerable<string> FilterWhiteSpaceAndDistinct(this IEnumerable<string>? sources)
    {
        if(sources.IsNullOrEmpty())
        {
            return Array.Empty<string>();
        }

        return sources!.Where(static x => !x.IsNullOrWhiteSpace())
            .Distinct();
    }

    public static bool ContainAny<T>(this IEnumerable<T> source, params T[] items)
    {
        if(source.IsNullOrEmpty()
            || items.Length == 0)
        {
            return false;
        }

        return source.Any(x => items.Contains(x));
    }

    public static bool ContainAll<T>(this IEnumerable<T> source, params T[] items)
    {
        if(source.IsNullOrEmpty()
            || items.Length == 0)
        {
            return false;
        }

        return source.Union(items).Count() == source.Count();
    }

    /// <summary>
    /// 先尝试从<see cref="Enumerable.TryGetNonEnumeratedCount{TSource}(IEnumerable{TSource}, out int)"/>中获取长度，如果没有成功，则使用<see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static int GetCount<T>(this IEnumerable<T> source)
    {
        if(source.TryGetNonEnumeratedCount(out var count))
        {
            return count;
        }

        return source.Count();
    }
}
