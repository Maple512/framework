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

namespace OneF.Moduleable.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

public class ServiceRegistrarHelper_Test
{
    private static readonly MethodInfo _registerTypeMethod = typeof(AssemblyScanHelper).GetMethod(
            "RegisterType",
            BindingFlagExtensions.StaticFlags)!;

    private static readonly MethodInfo _registerTypesMethod = typeof(AssemblyScanHelper).GetMethod(
            "RegisterTypes",
            BindingFlagExtensions.StaticFlags)!;

    [Theory]
    [InlineData(typeof(TestService1))]
    [InlineData(typeof(TestService2))]
    public void Register_type(Type type)
    {
        var serviecs = new ServiceCollection();

        _ = _registerTypeMethod.Invoke(null, new object[] { serviecs, type });

        serviecs.Count.ShouldBe(1);
    }

    [Theory]
    [InlineData(typeof(TestService1), typeof(TestService2))]
    [InlineData(typeof(TestService3), typeof(TestService2), typeof(TestService1))]
    public void Resigter_types(params Type[] types)
    {
        var serviecs = new ServiceCollection();

        var parameters = new List<object>
        {
            serviecs,
            types
        };

        _ = _registerTypesMethod.Invoke(null, parameters.ToArray());

        serviecs.Count.ShouldBe(types.Length);
    }

    [Theory]
    [InlineData(typeof(TestService2), typeof(TestService2))]
    [InlineData(typeof(TestService1), typeof(TestService1), typeof(TestService1))]
    public void Resigter_repeat_types(params Type[] types)
    {
        var serviecs = new ServiceCollection();

        var parameters = new List<object>
        {
            serviecs,
            types
        };

        _ = _registerTypesMethod.Invoke(null, parameters.ToArray());

        serviecs.Count.ShouldBe(types.Length);
    }

    [Fact]
    public void Register_self()
    {
        var serviecs = new ServiceCollection();

        var parameters = new List<object>
        {
            serviecs,
            new[] { typeof(TestService4), typeof(TestService5) }
        };

        _ = _registerTypesMethod.Invoke(null, parameters.ToArray());

        serviecs.Count.ShouldBe(2);

        // only self
        _ = serviecs.FirstOrDefault(x => x.ServiceType == typeof(TestService4)).ShouldNotBeNull();

        // not include self
        _ = serviecs.FirstOrDefault(x => x.ServiceType == typeof(ITestService5)).ShouldNotBeNull();
        _ = serviecs.FirstOrDefault(x => x.ImplementationType == typeof(TestService5)).ShouldNotBeNull();
    }

    [ServiceDescribe(ServiceLifetime.Singleton)]
    private class TestService1
    { }

    [ServiceDescribe(ServiceLifetime.Scoped)]
    private class TestService2
    { }

    [ServiceDescribe(ServiceLifetime.Transient)]
    private class TestService3
    { }

    private class TestService4 : ISingletonService
    { }

    private class TestService5 : ITestService5, ISingletonService
    { }

    public interface ITestService5
    { }
}
