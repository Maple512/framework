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
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using Microsoft.Extensions.DependencyInjection;

[MemoryDiagnoser, RankColumn, NativeMemoryProfiler, ThreadingDiagnoser, DisassemblyDiagnoser]
public class CreaetNewObject_Test
{
    private readonly ConstructorInfo _ctor;
    private readonly IServiceProvider _provider;
    private readonly Func<Employee> _expressionActivator;
    private readonly Func<Employee> _emitActivator;
    private readonly Func<Employee> _natashaActivator;

    public CreaetNewObject_Test()
    {
        _ctor = typeof(Employee).GetConstructor(Type.EmptyTypes);

        _provider = new ServiceCollection().AddTransient<Employee>().BuildServiceProvider();

        _ = NatashaInitializer.Initialize();
        _natashaActivator = Natasha.CSharp.NInstance.Creator<Employee>();

        _expressionActivator = Expression.Lambda<Func<Employee>>(Expression.New(typeof(Employee))).Compile();

        // il
        var dynamic = new DynamicMethod("DynamicMethod", typeof(Employee), null, typeof(CreaetNewObject_Test).Module, false);
        var il = dynamic.GetILGenerator();
        il.Emit(OpCodes.Newobj, typeof(Employee).GetConstructor(Type.EmptyTypes));
        il.Emit(OpCodes.Ret);
        _emitActivator = dynamic.CreateDelegate(typeof(Func<Employee>)) as Func<Employee>;
    }

    [Benchmark(Baseline = true)]
    public Employee UseNew() => new();

    [Benchmark]
    public Employee UseReflection() => _ctor.Invoke(null) as Employee;

    [Benchmark]
    public Employee UseActivator() => Activator.CreateInstance<Employee>();

    [Benchmark]
    public Employee UseDependencyInjection() => _provider.GetRequiredService<Employee>();

    [Benchmark]
    public Employee UseNatasha() => _natashaActivator();

    [Benchmark]
    public Employee UseExpression() => _expressionActivator();

    [Benchmark]
    public Employee UseEmit() => _emitActivator();
}

public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public string Descriptin { get; set; }
}
