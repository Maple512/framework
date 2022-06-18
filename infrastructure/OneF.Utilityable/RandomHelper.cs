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
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

[DebuggerStepThrough]
public static class RandomHelper
{
    public static int GetRandom(int minValue, int maxValue)
    {
        return RandomNumberGenerator.GetInt32(minValue, maxValue);
    }

    public static int GetRandom(int maxValue)
    {
        return RandomNumberGenerator.GetInt32(maxValue);
    }

    public static Span<byte> GetRandomBytes(int length)
    {
        Span<byte> bytes = new byte[length];

        RandomNumberGenerator.Fill(bytes);

        return bytes;
    }

    public static T GetRandomOf<T>(params T[] objs)
    {
        _ = Check.NotNullOrEmpty(objs);

        return objs[GetRandom(0, objs.Length)];
    }

    public static T GetRandomOfList<T>(IEnumerable<T> list)
    {
        _ = Check.NotNullOrEmpty(list);

        return list.ElementAt(GetRandom(0, list.Count()));
    }

    public static List<T> GenerateRandomizedList<T>(IEnumerable<T> items)
    {
        _ = Check.NotNull(items);

        var currentList = new List<T>(items);
        var randomList = new List<T>();

        while(currentList.Any())
        {
            var randomIndex = GetRandom(0, currentList.Count);

            randomList.Add(currentList[randomIndex]);

            currentList.RemoveAt(randomIndex);
        }

        return randomList;
    }
}
