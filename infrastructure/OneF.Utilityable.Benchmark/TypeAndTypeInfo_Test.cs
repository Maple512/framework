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
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

[MemoryDiagnoser, RankColumn, NativeMemoryProfiler, ThreadingDiagnoser, DisassemblyDiagnoser]
public class TypeAndTypeInfo_Test
{
    [Params(1, 5, 10, 100, 1000)]
    public int Count;

    [Benchmark(Baseline = true)]
    public List<string> Type_Reflecation()
    {
        return Enumerable.Range(0, Count)
            .Select(x => typeof(BigClass).GetInterfaces())
            .SelectMany(x => x.Select(t => t.AssemblyQualifiedName!))
            .ToList();
    }

    [Benchmark]
    public List<string> TypeInfo_Reflecation()
    {
        return Enumerable.Range(0, Count)
            .Select(x => typeof(BigClass).GetTypeInfo().GetInterfaces())
            .SelectMany(x => x.Select(t => t.AssemblyQualifiedName!))
            .ToList();
    }

    private interface IInterface0
    { }

    private interface IInterface1
    { }

    private interface IInterface2
    { }

    private interface IInterface3
    { }

    private interface IInterface4
    { }

    private interface IInterface5
    { }

    private interface IInterface6
    { }

    private interface IInterface7
    { }

    private interface IInterface8
    { }

    private interface IInterface9
    { }

    private interface IInterface10
    { }

    private interface IInterface11
    { }

    private interface IInterface12
    { }

    private interface IInterface13
    { }

    private interface IInterface14
    { }

    private interface IInterface15
    { }

    private interface IInterface16
    { }

    private interface IInterface17
    { }

    private interface IInterface18
    { }

    private interface IInterface19
    { }

    private interface IInterface20
    { }

    private interface IInterface21
    { }

    private interface IInterface22
    { }

    private interface IInterface23
    { }

    private interface IInterface24
    { }

    private interface IInterface25
    { }

    private interface IInterface26
    { }

    private interface IInterface27
    { }

    private interface IInterface28
    { }

    private interface IInterface29
    { }

    private interface IInterface30
    { }

    private interface IInterface31
    { }

    private interface IInterface32
    { }

    private interface IInterface33
    { }

    private interface IInterface34
    { }

    private interface IInterface35
    { }

    private interface IInterface36
    { }

    private interface IInterface37
    { }

    private interface IInterface38
    { }

    private interface IInterface39
    { }

    public class MethodsClass
    {
        public string Method0()
        { return ""; }

        public string Method1()
        { return ""; }

        public string Method2()
        { return ""; }

        public string Method3()
        { return ""; }

        public string Method4()
        { return ""; }

        public string Method5()
        { return ""; }

        public string Method6()
        { return ""; }

        public string Method7()
        { return ""; }

        public string Method8()
        { return ""; }

        public string Method9()
        { return ""; }

        public string Method10()
        { return ""; }

        public string Method11()
        { return ""; }

        public string Method12()
        { return ""; }

        public string Method13()
        { return ""; }

        public string Method14()
        { return ""; }

        public string Method15()
        { return ""; }

        public string Method16()
        { return ""; }

        public string Method17()
        { return ""; }

        public string Method18()
        { return ""; }

        public string Method19()
        { return ""; }

        public string Method20()
        { return ""; }

        public string Method21()
        { return ""; }

        public string Method22()
        { return ""; }

        public string Method23()
        { return ""; }

        public string Method24()
        { return ""; }

        public string Method25()
        { return ""; }

        public string Method26()
        { return ""; }

        public string Method27()
        { return ""; }

        public string Method28()
        { return ""; }

        public string Method29()
        { return ""; }
    }

    private class BigClass : MethodsClass,
        IInterface0,
IInterface1,
IInterface2,
IInterface3,
IInterface4,
IInterface5,
IInterface6,
IInterface7,
IInterface8,
IInterface9,
IInterface10,
IInterface11,
IInterface12,
IInterface13,
IInterface14,
IInterface15,
IInterface16,
IInterface17,
IInterface18,
IInterface19,
IInterface20,
IInterface21,
IInterface22,
IInterface23,
IInterface24,
IInterface25,
IInterface26,
IInterface27,
IInterface28,
IInterface29,
IInterface30,
IInterface31,
IInterface32,
IInterface33,
IInterface34,
IInterface35,
IInterface36,
IInterface37,
IInterface38,
IInterface39

    {
        public int Id { get; set; }
        public string Name { get; }
        public string Description { get; }
        public int Age { get; }
        public string Password { get; }
    }
}
