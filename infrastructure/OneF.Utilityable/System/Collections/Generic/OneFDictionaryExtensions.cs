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
using OneF;

[StackTraceHidden]
[DebuggerStepThrough]
public static class OneFDictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        TValue value)
    {
        return GetOrAdd(directory, key, () => value);
    }

    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        Func<TValue> factory)
    {
        return GetOrAdd(directory, key, _ => factory());
    }

    public static TValue GetOrAdd<TKey, TValue>(
        this IDictionary<TKey, TValue> directory,
        TKey key,
        Func<TKey, TValue> factory)
    {
        _ = Check.NotNull(key);

        if(directory.TryGetValue(key, out var result))
        {
            return result;
        }

        return directory[key] = factory(key);
    }

    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }
}
