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
public static class OneFCollectionExtensions
{
    public static void AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        _ = Check.NotNull(source);

        if(!source.Contains(item))
        {
            source.Add(item);
        }
    }

    public static void AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        _ = Check.NotNull(source);

        foreach(var item in items)
        {
            if(!source.Contains(item))
            {
                source.Add(item);
            }
        }
    }

    public static IEnumerable<T> RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        var items = source.Where(predicate).ToArray();

        foreach(var item in items)
        {
            _ = source.Remove(item);
        }

        return items;
    }
}
