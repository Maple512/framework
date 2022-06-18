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

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

/*
 获取集合长度时的性能对比
 TryGetNonEnumeratedCount > Count > ToArray > ToList
 */
[MemoryDiagnoser, RankColumn, NativeMemoryProfiler, ThreadingDiagnoser, DisassemblyDiagnoser]
public class EnumerableCount_Test
{
    private readonly IEnumerable<string> source;

    public EnumerableCount_Test()
    {
        source = Enumerable.Range(0, 10_0000).Select(x => $"item{x}");
    }

    [Benchmark(Baseline = true)]
    public int UseCount() => source.Count();

    [Benchmark]
    public int UseToArray()
    {
        var arr = source.ToArray();

        return arr.Length;
    }

    [Benchmark]
    public int UseToList()
    {
        var arr = source.ToList();

        return arr.Count;
    }

    [Benchmark]
    public int UseTry()
    {
        _ = source.TryGetNonEnumeratedCount(out var count);

        return count;
    }
}
